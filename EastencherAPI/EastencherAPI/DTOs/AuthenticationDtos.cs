using AuthApplication.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApplication.DTOs
{
    public class LoginModel
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string clientId { get; set; }
    }

    public class ClientIdModel
    {
        public Guid UserId { get; set; }
        public string clientId { get; set; }
    }

    public class AuthenticationResult
    {
        public Guid UserID { get; set; }
        public string EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string UserRole { get; set; }
        public string Token { get; set; }
        public List<App> MenuItemNames { get; set; }
        public string IsChangePasswordRequired { get; set; }
        public string ReasonForReset { get; set; }
        public string? ProfilePic { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? Designation { get; set; }
        public string? Division { get; set; }
        public string? Department { get; set; }
    }

    public class LoginResult
    {
        public Guid UserID { get; set; }
        public string EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string UserRole { get; set; }
        public string Token { get; set; }
        public string IsChangePasswordRequired { get; set; }
        public string ReasonForReset { get; set; }
        public string? ProfilePic { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? Designation { get; set; }
        public string? Division { get; set; }
    }
}