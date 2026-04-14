using AuthApplication.DTOs;  // NEW: Add DTO import
using AuthApplication.Models;
using AuthApplication.Services;
using EastencherAPI.DBContext;
using EastencherAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static AuthApplication.Services.AuthService;


namespace AuthApplication.Controllers
{
    [ApiController]
    [Route("api/AuthMasterController")]
    public class AuthMasterController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly AuthMasterServices _authMasterService;
        private readonly int _tokenTimespan;

        public AuthMasterController(AppDbContext dbContext, IConfiguration configuration, AuthMasterServices authMasterService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authMasterService = authMasterService;
            try
            {
                var span = "30";
                if (span != "")
                    _tokenTimespan = Convert.ToInt32(span.ToString());
                if (_tokenTimespan <= 0)
                {
                    _tokenTimespan = 30;
                }
            }
            catch
            {
                _tokenTimespan = 30;
            }
        }

        #region Authentication

        [HttpGet("FindClient")]
        public IActionResult FindClient(string clientId)
        {
            try
            {
                var client = _authMasterService.FindClient(clientId);
                if (client != null)
                {
                    return Json(new { success = "success", message = "You have successfully get data", data = client });
                }
                else
                {
                    return Json(new { success = "error", message = "Data Not Found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        #endregion

        #region UserControll

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers(string ClientId)
        {
            try
            {
                var users = _authMasterService.GetAllUsers(ClientId);
                return Json(new { success = "success", message = "You have successfully get user data", data = users });

            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpGet("GetUsersById")]
        public async Task<IActionResult> GetUsersById(string UserId)
        {
            try
            {
                var users = await _authMasterService.GetUsersById(UserId);

                return Json(new
                {
                    success = "success",
                    message = "You have successfully get user data",
                    data = users
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(UserWithRole userWithRole)
        {
            try
            {
                var userResult = await _authMasterService.CreateUser(userWithRole);
                return Json(new { success = "success", message = "You have successfully created the user."});
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("ProfileUpdateUser")]
        public async Task<ActionResult> ProfileUpdateUser([FromForm] UserProfileUpdate userProfileUpdate)
        {
            try
            {
                var userResult = await _authMasterService.ProfileUpdateUser(userProfileUpdate);
                return Json(new { success = "success", message = "Profile image updated successfully...", data = userResult });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("UpdateUser")]
        public async Task<ActionResult<UserWithRole>> UpdateUser(UserWithRole userWithRole)
        {
            try
            {
                var userResult = await _authMasterService.UpdateUser(userWithRole);
                return Json(new { success = "success", message = "Data updated successfully", data = userResult });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("DeleteUser")]
        public async Task<ActionResult<UserWithRole>> DeleteUser(string ClientId, Guid UserID)
        {
            try
            {
                var userResult = await _authMasterService.DeleteUser(ClientId, UserID);
                return Json(new { success = "success", message = "You have successfully Deleted User", data = userResult });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        #endregion

        #region RoleControll

        [HttpGet("GetAllRoles")]
        public IActionResult GetAllRoles(string ClientId)
        {
            try
            {
                var roles = _authMasterService.GetAllRoles(ClientId);
                return Json(new { success = "success", message = "You have successfully get the  Data", data = roles });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpGet("GetAllRolesWithApp")]
        public IActionResult GetAllRolesWithApp(string ClientId)
        {
            try
            {
                var rolesWithApp = _authMasterService.GetAllRolesWithApp(ClientId);
                return Json(new { success = "success", message = "You have successfully get the Data", data = rolesWithApp });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleWithApp roleWithApp)
        {
            try
            {
                var createdRole = await _authMasterService.CreateRole(roleWithApp);
                return Json(new { success = "success", message = "You have successfully Created Data", data = createdRole });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("UpdateRole")]
        public async Task<IActionResult> UpdateRole(RoleWithApp roleWithApp)
        {
            try
            {
                var updatedRole = await _authMasterService.UpdateRole(roleWithApp);
                return Json(new { success = "success", message = "You have successfully Update Data", data = updatedRole });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("DeleteRole")]
        public async Task<IActionResult> DeleteRole(Guid roleId)
        {
            try
            {
                var result = await _authMasterService.DeleteRole(roleId);
                return Json(new { success = "success", message = "You have successfully Deleted Data", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        #endregion

        #region AppControll

        [HttpGet("GetAllApps")]
        public IActionResult GetAllApps()
        {
            try
            {
                var apps = _authMasterService.GetAllApps();
                return Json(new { success = "success", message = "You have successfully Update Data", data = apps });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("CreateApp")]
        public async Task<IActionResult> CreateApp(App app)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new ArgumentException("Model state is not valid");
                }

                var createdApp = await _authMasterService.CreateApp(app);
                return Json(new { success = "success", message = "You have successfully Created Data", data = createdApp });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPut("UpdateApp")]
        public async Task<IActionResult> UpdateApp(App app)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new ArgumentException("Model state is not valid");
                }

                var updatedApp = await _authMasterService.UpdateApp(app);
                return Json(new { success = "success", message = "You have successfully Update Data", data = updatedApp });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("DeleteApp")]
        public async Task<IActionResult> DeleteApp(App app)
        {
            try
            {
                var deletedApp = await _authMasterService.DeleteApp(app);
                return Json(new { success = "success", message = "You have successfully Deleted App", data = deletedApp });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        #endregion

        #region LoginHistoryControll

        //[HttpPost("LoginHistory")]
        //public async Task<IActionResult> LoginHistory(Guid userID, string username)
        //{
        //    try
        //    {
        //        var loginData = await _authMasterService.AddLoginHistory(userID, username);
        //        //return Ok(loginData);
        //        return Json(new { success = "success", message = "You have successfully get user data", data = loginData });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = "error", message = ex.Message });
        //    }
        //}

        [HttpGet("GetAllUsersLoginHistory")]
        public IActionResult GetAllUsersLoginHistory()
        {
            try
            {
                var userLoginHistoryList = _authMasterService.GetAllUsersLoginHistory();
                //return Ok(userLoginHistoryList);
                return Json(new { success = "success", message = "You have successfully get user data", data = userLoginHistoryList });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpGet("GetCurrentUserLoginHistory")]
        public IActionResult GetCurrentUserLoginHistory(Guid userID)
        {
            try
            {
                var userLoginHistoryList = _authMasterService.GetCurrentUserLoginHistory(userID);
                //return Ok(userLoginHistoryList);
                return Json(new { success = "success", message = "You have successfully get user data", data = userLoginHistoryList });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut(Guid userID)
        {
            try
            {
                var result = await _authMasterService.SignOut(userID);
                //return Ok(result);
                return Json(new { success = "success", message = "You have successfully get user data", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        #endregion

        #region ChangePasswordControl

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePassword changePassword)
        {
            try
            {
                var result = await _authMasterService.ChangePassword(changePassword);
                //return Ok(result);
                return Json(new { success = "success", message = "You have successfully Reseted the Password", data = result });

            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("SendResetLinkToMail")]
        public async Task<ActionResult<AuthTokenHistory>> SendResetLinkToMail(EmailModel emailModel)
        {
            try
            {
                var result = await _authMasterService.SendResetLinkToMail(emailModel);
                //return Ok(result);
                return Json(new { success = "success", message = "You have successfully Sent Mail", data = result });

            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            try
            {
                var result = await _authMasterService.ForgotPassword(forgotPassword);
                //return Ok();
                return Json(new { success = "success", message = "You have successfully Reseted password", data = result });

            }
            catch (Exception ex)
            {
                return Json(new { success = "error", message = ex.Message });
            }
        }

        #endregion

        #region LogGetApi's

        //[HttpGet("GetUsersCLog")]
        //public ActionResult<List<ErrorLogEntry>> GetUsersCLog()
        //{
        //    var errorLogs = _authMasterService.GetUsersCLog();
        //    return Ok(errorLogs);
        //}

        //[HttpGet("GetAuthMasterErrorLog")]
        //public ActionResult<List<LoginLogEntry>> GetAuthMasterErrorLog()
        //{
        //    var loginLogs = _authMasterService.GetAuthMasterErrorLog();
        //    return Ok(loginLogs);
        //}

        #endregion

        [HttpPost("UpdateUserStatus")]
        public async Task<IActionResult> UpdateUserStatus(string userID, bool isActive, string modifiedBy, string? department)
        {
            try
            {
                var checkuser = await _dbContext.Users.Where(x => x.UserID.ToString().ToLower() == userID.ToLower()).FirstOrDefaultAsync();
                if (checkuser != null)
                {
                    checkuser.IsActive = isActive;
                    checkuser.ModifiedBy = modifiedBy;
                    checkuser.IsActive = isActive;
                }
                else
                {
                    return Ok(new { success = false, message = "User data not found", data = checkuser });
                }
                await _dbContext.SaveChangesAsync();
                return Ok(new { success = false, message = "User data updated successfully", data = checkuser });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        private string Decrypt(string Password, bool UseHashing)
        {
            try
            {
                string EncryptionKey = "Eastencher";
                byte[] KeyArray;
                byte[] ToEncryptArray = Convert.FromBase64String(Password);
                if (UseHashing)
                {
                    using (MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider())
                    {
                        KeyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
                    }
                }
                else
                {
                    KeyArray = Encoding.UTF8.GetBytes(EncryptionKey);
                }

                using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = KeyArray;
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = tdes.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(ToEncryptArray, 0, ToEncryptArray.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        //[HttpPost("CreateMasterData")]
        //public async Task<IActionResult> CreateMasterData(MasterDto data)
        //{
        //    var tranaction = await _dbContext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        if (string.IsNullOrEmpty(data.MasterName) || (data.MasterName) == "string")
        //        {
        //            return Ok(new { success = false, message = "Mastername is required" });
        //        }

        //        if(string.IsNullOrEmpty(data.MasterName))
        //        {
        //            var exist = await _dbContext.Masters
        //                .AnyAsync(x => x.MasterName.ToLower() == data.MasterName.ToLower() && x.MasterValue.ToLower() == data.MasterValue.ToLower());
        //            if(exist)
        //            {
        //                return Ok(new { success = false, message = "Data is already Exist" });
        //            }
        //        }

        //        var master = new MasterDto
        //        {
        //            MasterName = data.MasterName,
        //            MasterValue = data.MasterValue,
        //            CreatedBy = data.CreatedBy,
        //            CreatedOn = DateTime.Now,
        //            IsActive = true,

        //        };

        //        await tranaction.CommitAsync();
        //        return Ok(new { success = true, message = "Data Created Successfully", data = master });

        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpGet("GetAllMasterData")]
        //public async Task<IActionResult> GetAllMasterData()
        //{
        //    try
        //    {
        //        var data = await _dbContext.Masters.ToListAsync();
        //        return Ok(new { success = true, message = "Data extarced successfully", data = data });

        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(new { sucess = false, message = ex.Message });
        //    }
        //}

        #region Register API

        [HttpPost("Register")]
        [AllowAnonymous]  // Allow anonymous access for registration
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
                    return Json(new
                    {
                        success = "error",
                        message = "Validation failed",
                        errors = errors
                    });
                }

                // Call service to register user
                var result = await _authMasterService.RegisterUser(registerDto);

                return Json(new
                {
                    success = "success",
                    message = result.Message,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new
                {
                    success = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = "error",
                    message = ex.Message ?? "An unexpected error occurred during registration"
                });
            }
        }

        #endregion

        //#region Institution Master Related API's

        //[HttpPost("CreateOrUpdateInstitution")]
        //public async Task<IActionResult> CreateOrUpdateInstitution(InstitutionDto data)
        //{
        //    try
        //    {
        //        // Validation: If InstitutionName is provided, check for duplicates (case-insensitive) for active Institution
        //        var existingInstitution = await _dbContext.Institution
        //            .FirstOrDefaultAsync(x => x.InstitutionName != null && x.InstitutionName.ToLower().Trim() == data.InstitutionName.ToLower().Trim() && x.InstitutionCode.ToLower().Trim() == data.InstitutionCode.ToLower().Trim() && x.IsActive == true);

        //        if (existingInstitution != null && existingInstitution.InstitutionId != data.InstitutionId)
        //        {
        //            return Ok(new { success = false, message = $"Institution with name '{data.InstitutionName}' already exists." });
        //        }

        //        // If the Institution exists → Update
        //        if (data.InstitutionId > 0)
        //        {
        //            var institutionToUpdate = await _dbContext.Institution
        //                .FirstOrDefaultAsync(x => x.InstitutionId == data.InstitutionId);

        //            if (institutionToUpdate == null)
        //            {
        //                return Ok(new { success = false, message = $"Institution with Id {data.InstitutionId} not found." });
        //            }

        //            // Update fields if necessary
        //            List<string> updatedFields = new List<string>();

        //            void UpdateField<T>(string fieldName, T existingValue, T newValue, Action<T> applyChange)
        //            {
        //                if (!EqualityComparer<T>.Default.Equals(existingValue, newValue))
        //                {
        //                    applyChange(newValue);
        //                    updatedFields.Add($"{fieldName}: Existing Data : \"{existingValue}\" Updated to \"{newValue}\"");
        //                }
        //            }

        //            UpdateField("InstitutionName", institutionToUpdate.InstitutionName, data.InstitutionName, val => institutionToUpdate.InstitutionName = val);
        //            UpdateField("InstitutionCode", institutionToUpdate.InstitutionCode, data.InstitutionCode, val => institutionToUpdate.InstitutionCode = val);
        //            UpdateField("IsActive", institutionToUpdate.IsActive, data.IsActive, val => institutionToUpdate.IsActive = val);

        //            institutionToUpdate.ModifiedOn = DateTime.Now;
        //            institutionToUpdate.ModifiedBy = data.UserId;

        //            _dbContext.Institution.Update(institutionToUpdate);

        //            if (updatedFields.Any())
        //            {
        //                Log.DataLog($"{data.UserId}",
        //                    $"Institution Id {data.InstitutionId} updated fields: {string.Join(", ", updatedFields)}",
        //                    "Institution");
        //            }

        //            await _dbContext.SaveChangesAsync();

        //            return Ok(new { success = true, message = "Institution updated successfully.", data = data });
        //        }
        //        else
        //        {
        //            // If Institution does not exist → Create new
        //            var newInstitution = new Institution
        //            {
        //                InstitutionName = data.InstitutionName,
        //                InstitutionCode = data.InstitutionCode,
        //                IsActive = true,
        //                CreatedBy = data.UserId,
        //                CreatedOn = DateTime.Now,
        //            };

        //            await _dbContext.Institution.AddAsync(newInstitution);
        //            await _dbContext.SaveChangesAsync();

        //            Log.DataLog($"{data.UserId}",
        //                $"Institution created with Name {data.InstitutionName} and Code {data.InstitutionCode}",
        //                "Institution");

        //            return Ok(new { success = true, message = "Institution created successfully.", data = data });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpGet("GetAllInstitutions")]
        //public async Task<IActionResult> GetAllInstitutions()
        //{
        //    try
        //    {
        //        var institutions = await _dbContext.Institution
        //            .Where(x => x.IsActive == true)
        //            .OrderByDescending(x => x.CreatedOn)
        //            .ToListAsync();

        //        return Ok(new { success = true, message = "Institutions data fetched successfully", data = institutions });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpGet("GetInstitutionById")]
        //public async Task<IActionResult> GetInstitutionById(int id)
        //{
        //    try
        //    {
        //        if (id <= 0)
        //        {
        //            return Ok(new { success = false, message = "Invalid Institution Id." });
        //        }

        //        var institution = await _dbContext.Institution
        //            .FirstOrDefaultAsync(x => x.InstitutionId == id && x.IsActive == true);

        //        if (institution == null)
        //        {
        //            return Ok(new { success = false, message = $"Institution with Id {id} not found." });
        //        }

        //        return Ok(new { success = true, message = "Institution data fetched successfully", data = institution });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpPost("DeleteInstitutionById")]
        //public async Task<IActionResult> DeleteInstitutionById(int id, string? UserId)
        //{
        //    try
        //    {
        //        if (id <= 0)
        //        {
        //            return Ok(new { success = false, message = "Valid Institution Id is required" });
        //        }
        //        if (string.IsNullOrWhiteSpace(UserId))
        //        {
        //            return Ok(new { success = false, message = "UserId is required" });
        //        }

        //        var userExists = await _dbContext.Users.AnyAsync(x => x.UserID.ToString().ToLower() == UserId.ToString() && x.IsActive == true);
        //        if (!userExists)
        //        {
        //            return Ok(new { success = false, message = "UserId Not Found" });
        //        }

        //        var institution = await _dbContext.Institution.FirstOrDefaultAsync(x => x.InstitutionId == id);
        //        if (institution == null)
        //        {
        //            return Ok(new { success = false, message = "Institution not found" });
        //        }

        //        _dbContext.Institution.Remove(institution);
        //        await _dbContext.SaveChangesAsync();

        //        Log.DataLog(UserId, $"Institution with Id {id} deleted. Name: '{institution.InstitutionName}', Code: '{institution.InstitutionCode}'", "Institution");

        //        return Ok(new { success = true, message = "Institution deleted successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}


        //#endregion
    }

}

