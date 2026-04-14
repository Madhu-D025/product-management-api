using AuthApplication.DTOs;
using AuthApplication.Models;
using EastencherAPI.DBContext;
using EastencherAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EastencherAPI.Services
{
    public class ProductServices
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public ProductServices(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        #region Helper Methods

        private async Task<User> ValidateUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("UserId is required");
            }

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserID.ToString().ToLower() == userId.ToLower() && x.IsActive);

            if (user == null)
            {
                throw new Exception("UserId Not Found");
            }

            return user;
        }

        private async Task<bool> IsAdminUserAsync(string userId)
        {
            try
            {
                var adminRole = await _dbContext.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "Admin" && r.IsActive);

                if (adminRole == null)
                {
                    return false;
                }

                var userRole = await _dbContext.UserRoleMaps
                    .FirstOrDefaultAsync(ur => ur.UserID.ToString() == userId && ur.RoleID == adminRole.RoleID && ur.IsActive);

                return userRole != null;
            }
            catch (Exception ex)
            {
                Log.Error(userId, $"Error validating admin role: {ex.Message}", "Product");
                return false;
            }
        }

        private async Task<bool> ProductExistsByNameAsync(string productName, int? excludeProductId = null)
        {
            var query = _dbContext.Products.AsQueryable();

            if (excludeProductId.HasValue && excludeProductId.Value > 0)
            {
                query = query.Where(p => p.Id != excludeProductId.Value);
            }

            var exists = await query
                .AnyAsync(p => p.Name != null && p.Name.ToLower().Trim() == productName.ToLower().Trim() && p.IsActive == true);

            return exists;
        }

        #endregion

        #region Create Product

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate user exists and is active (AUTHENTICATION)
                var user = await ValidateUserAsync(productDto.UserId);

                // 2. Validate user is Admin (AUTHORIZATION)
                bool isAdmin = await IsAdminUserAsync(user.UserID.ToString());
                if (!isAdmin)
                {
                    throw new Exception("Only Admin users can create products");
                }

                // 3. Validate input
                if (string.IsNullOrWhiteSpace(productDto.Name))
                {
                    throw new Exception("Product Name is required");
                }

                if (productDto.Price <= 0 || productDto.Price == null)
                {
                    throw new Exception("Product Price must be greater than 0");
                }

                // 4. Check for duplicate product name
                bool productExists = await ProductExistsByNameAsync(productDto.Name);
                if (productExists)
                {
                    throw new Exception($"Product with name '{productDto.Name}' already exists");
                }

                // 5. Create product entity
                var newProduct = new Product
                {
                    Name = productDto.Name.Trim(),
                    Description = productDto.Description?.Trim(),
                    Price = productDto.Price,
                    IsActive = true,
                    CreatedBy = user.UserID.ToString(),
                    CreatedOn = DateTime.Now
                };

                // 6. Add to database
                _dbContext.Products.Add(newProduct);
                await _dbContext.SaveChangesAsync();

                Log.DataLog(
                    user.UserID.ToString(),
                    $"Product created successfully: Name='{newProduct.Name}', Price={newProduct.Price}",
                    "Product");

                // 7. Map to DTO and return
                var responseDto = new ProductDto
                {
                    Id = newProduct.Id,
                    Name = newProduct.Name,
                    Description = newProduct.Description,
                    Price = newProduct.Price,
                    IsActive = newProduct.IsActive,
                    UserId = user.UserID.ToString()
                };

                await transaction.CommitAsync();
                return responseDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(productDto.UserId, $"Product creation failed: {ex.Message}", "Product");
                throw;
            }
        }

        #endregion

        #region Update Product

        public async Task<ProductDto> UpdateProductAsync(ProductDto productDto)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate user exists and is active (AUTHENTICATION)
                var user = await ValidateUserAsync(productDto.UserId);

                // 2. Validate user is Admin (AUTHORIZATION)
                bool isAdmin = await IsAdminUserAsync(user.UserID.ToString());
                if (!isAdmin)
                {
                    throw new Exception("Only Admin users can update products");
                }

                // 3. Validate input
                if (productDto.Id <= 0)
                {
                    throw new Exception("Valid Product ID is required");
                }

                if (string.IsNullOrWhiteSpace(productDto.Name))
                {
                    throw new Exception("Product Name is required");
                }

                if (productDto.Price <= 0 || productDto.Price == null)
                {
                    throw new Exception("Product Price must be greater than 0");
                }

                // 4. Find product to update
                var productToUpdate = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == productDto.Id);

                if (productToUpdate == null)
                {
                    throw new Exception($"Product with Id {productDto.Id} not found");
                }

                // 5. Check for duplicate product name (excluding current product)
                bool productNameExists = await ProductExistsByNameAsync(productDto.Name, productDto.Id);
                if (productNameExists)
                {
                    throw new Exception($"Product with name '{productDto.Name}' already exists");
                }

                // 6. Track changes for audit logging
                List<string> updatedFields = new List<string>();

                void UpdateField<T>(string fieldName, T existingValue, T newValue, Action<T> applyChange)
                {
                    if (!EqualityComparer<T>.Default.Equals(existingValue, newValue))
                    {
                        applyChange(newValue);
                        updatedFields.Add($"{fieldName}: Existing Data : \"{existingValue}\" Updated to \"{newValue}\"");
                    }
                }

                UpdateField("Name", productToUpdate.Name, productDto.Name.Trim(), val => productToUpdate.Name = val);
                UpdateField("Description", productToUpdate.Description, productDto.Description?.Trim(), val => productToUpdate.Description = val);
                UpdateField("Price", productToUpdate.Price, productDto.Price, val => productToUpdate.Price = val);
                UpdateField("IsActive", productToUpdate.IsActive, productDto.IsActive, val => productToUpdate.IsActive = val);

                productToUpdate.ModifiedOn = DateTime.Now;
                productToUpdate.ModifiedBy = user.UserID.ToString();

                // 7. Update database
                _dbContext.Products.Update(productToUpdate);
                await _dbContext.SaveChangesAsync();

                if (updatedFields.Any())
                {
                    Log.DataLog(
                        user.UserID.ToString(),
                        $"Product Id {productDto.Id} updated fields: {string.Join(", ", updatedFields)}",
                        "Product");
                }

                // 8. Map to DTO and return
                var responseDto = new ProductDto
                {
                    Id = productToUpdate.Id,
                    Name = productToUpdate.Name,
                    Description = productToUpdate.Description,
                    Price = productToUpdate.Price,
                    IsActive = productToUpdate.IsActive,
                    UserId = user.UserID.ToString()
                };

                await transaction.CommitAsync();
                return responseDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(productDto.UserId, $"Product update failed: {ex.Message}", "Product");
                throw;
            }
        }

        #endregion

        #region Delete Product
        public async Task<ProductDto> DeleteProductAsync(int productId, string userId)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate user exists and is active (AUTHENTICATION)
                var user = await ValidateUserAsync(userId);

                // 2. Validate user is Admin (AUTHORIZATION)
                bool isAdmin = await IsAdminUserAsync(user.UserID.ToString());
                if (!isAdmin)
                {
                    throw new Exception("Only Admin users can delete products");
                }

                // 3. Validate Product ID
                if (productId <= 0)
                {
                    throw new Exception("Valid Product ID is required");
                }

                // 4. Find product
                var productToDelete = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (productToDelete == null)
                {
                    throw new Exception($"Product with ID {productId} not found");
                }

                // 5. Store product details for logging before deletion
                string productName = productToDelete.Name;

                // 6. Perform permanent delete (remove from database)
                _dbContext.Products.Remove(productToDelete);
                await _dbContext.SaveChangesAsync();

                Log.DataLog(
                    user.UserID.ToString(),
                    $"Product ID {productId} ('{productName}') permanently deleted from database",
                    "Product");

                // 7. Map to DTO and return (return the deleted product data)
                var responseDto = new ProductDto
                {
                    Id = productToDelete.Id,
                    Name = productToDelete.Name,
                    Description = productToDelete.Description,
                    Price = productToDelete.Price,
                    IsActive = false,
                    UserId = user.UserID.ToString()
                };

                await transaction.CommitAsync();
                return responseDto;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(userId, $"Product deletion failed: {ex.Message}", "Product");
                throw;
            }
        }


        #endregion

        #region Get All Products
        public async Task<List<ProductDto>> GetAllProductsAsync(string userId)
        {
            try
            {
                // 1. Validate user exists and is active (AUTHENTICATION)
                var user = await ValidateUserAsync(userId);

                // 2. Fetch all active products
                var products = await _dbContext.Products
                    .Where(p => p.IsActive == true)
                    .OrderByDescending(p => p.CreatedOn)
                    .ToListAsync();

                // 3. Map to DTO list
                var responseList = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    UserId = user.UserID.ToString()
                }).ToList();

                Log.DataLog(user.UserID.ToString(), $"Retrieved {responseList.Count} products", "Product");

                return responseList;
            }
            catch (Exception ex)
            {
                Log.Error(userId, $"Failed to retrieve products: {ex.Message}", "Product");
                throw;
            }
        }

        #endregion

        #region Get Product by ID
        public async Task<ProductDto> GetProductByIdAsync(int productId, string userId)
        {
            try
            {
                // 1. Validate user exists and is active (AUTHENTICATION)
                var user = await ValidateUserAsync(userId);

                // 2. Validate Product ID
                if (productId <= 0)
                {
                    throw new Exception("Valid Product ID is required");
                }

                // 3. Fetch product
                var product = await _dbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive == true);

                if (product == null)
                {
                    throw new Exception($"Product with ID {productId} not found");
                }

                // 4. Map to DTO and return
                var responseDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    IsActive = product.IsActive,
                    UserId = user.UserID.ToString()
                };

                Log.DataLog(user.UserID.ToString(), $"Retrieved product ID {productId}", "Product");

                return responseDto;
            }
            catch (Exception ex)
            {
                Log.Error(userId, $"Failed to retrieve product: {ex.Message}", "Product");
                throw;
            }
        }

        #endregion
    }
}