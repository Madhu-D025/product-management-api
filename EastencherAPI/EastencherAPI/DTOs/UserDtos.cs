using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApplication.DTOs
{
    public class UserWithRole
    {
        public Guid? UserID { get; set; }
        public Guid RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; }
        public bool? EmailStatus { get; set; }
        public string? ClientId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ProfilePath { get; set; }
        public string? PicDbPath { get; set; }
        public IFormFile? ProfilePic { get; set; }
    }

    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(128, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12,}$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string? FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Client ID is required")]
        public string? ClientId { get; set; }
    }

    public class RegisterResponse
    {
        public Guid UserID { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; }
        public bool EmailStatus { get; set; }
        public string? RoleName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Message { get; set; } = "User registration successful";
    }

    public class UserReffdata
    {
        public Guid? UserID { get; set; }
        public Guid RoleID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Division { get; set; }
        public string RoleName { get; set; }
        public string? ClientId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserProfileUpdate
    {
        public Guid? UserID { get; set; }
        public string? ModifiedBy { get; set; }
        public IFormFile? ProfilePic { get; set; }
    }

    public class UserView
    {
        public Guid UserID { get; set; }
        public string EmployeeId { get; set; }
    }

    public class UpdateActiveInactiveModel
    {
        public Guid UserID { get; set; }
        public bool IsActive { get; set; }
    }

    public class VendorUser
    {
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class EmpInfo
    {
        [Key]
        public Guid EmpId { get; set; }
        public int? EmpInfoId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Department { get; set; }
        public string? JoiningDate { get; set; }
        public string? Plant { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class FromEmployeeEmpInfo
    {
        public Guid EmpId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class ManagerUserMapDTO
    {
        public Guid ManagerID { get; set; }
        public string? UserID { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class ManagerUserMapData
    {
        public Guid ManagerID { get; set; }
        public string? UserID { get; set; }
    }
}