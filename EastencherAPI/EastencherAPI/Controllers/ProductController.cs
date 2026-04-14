using AuthApplication.DTOs;
using EastencherAPI.Models;
using EastencherAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EastencherAPI.Controllers
{

    [ApiController]
    [Route("api/ProductController")]
    [Authorize]  // Require authentication for all endpoints
    public class ProductController : Controller
    {
        private readonly ProductServices _productService;

        public ProductController(ProductServices productService)
        {
            _productService = productService;
        }

        #region Create Product

        //[HttpPost("CreateOrUpdateProduct")]
        //public async Task<IActionResult> CreateOrUpdateProduct(ProductDto data)
        //{
        //    try
        //    {
        //        if (data.Id > 0)
        //        {
        //            // If the Product exists ? Update
        //            var productResult = await _productService.UpdateProductAsync(data);
        //            return Ok(new { success = true, message = "Product updated successfully.", data = productResult });
        //        }
        //        else
        //        {
        //            // If Product does not exist ? Create new
        //            var productResult = await _productService.CreateProductAsync(data);
        //            return Ok(new { success = true, message = "Product created successfully.", data = productResult });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(ProductDto data)
        {
            try
            {
                var productResult = await _productService.CreateProductAsync(data);
                return Ok(new { success = true, message = "Product created successfully.", data = productResult });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(ProductDto data)
        {
            try
            {
                
                    var productResult = await _productService.UpdateProductAsync(data);
                    return Ok(new { success = true, message = "Product updated successfully.", data = productResult });
              
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Get All Products
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(string userId)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(userId);
                return Ok(new { success = true, message = "Products data fetched successfully", data = products });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Get Product by ID
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int id, string userId)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(new { success = false, message = "Invalid Product Id." });
                }

                var product = await _productService.GetProductByIdAsync(id, userId);

                if (product == null)
                {
                    return Ok(new { success = false, message = $"Product with Id {id} not found." });
                }

                return Ok(new { success = true, message = "Product data fetched successfully", data = product });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Delete Product
        [HttpPost("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id, string userId)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(new { success = false, message = "Valid Product Id is required" });
                }
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Ok(new { success = false, message = "UserId is required" });
                }

                var product = await _productService.DeleteProductAsync(id, userId);

                return Ok(new { success = true, message = "Product deleted successfully", data = product });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}