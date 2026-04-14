using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApplication.Models
{
    /// <summary>
    /// Entity Models - Use for database entities only
    /// DTOs have been moved to DTOs folder
    /// </summary>

    public class User
    {
        [Key]
        public Guid UserID { get; set; }
        public string? RoleID { get; set; }
        public string? RoleName { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; }
        public bool? EmailStatus { get; set; }
        public string? Password { get; set; }
        public string? Pass1 { get; set; }
        public string? Pass2 { get; set; }
        public string? Pass3 { get; set; }
        public string? Pass4 { get; set; }
        public string? Pass5 { get; set; }
        public string? LastChangedPassword { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? IsLockDuration { get; set; }
        public int Attempts { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ClientId { get; set; }
        public string? ProfilePath { get; set; }
        public string? PicDbPath { get; set; }
    }

    public class Role
    {
        [Key]
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ClientId { get; set; }
    }

    public class UserRoleMap
    {
        [Column(Order = 0), Key, ForeignKey("User")]
        public Guid UserID { get; set; }
        [Column(Order = 1), Key, ForeignKey("Role")]
        public Guid RoleID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class App
    {
        [Key]
        public int AppID { get; set; }
        public string AppName { get; set; }
        public string? AppRoute { get; set; }
        public string? ClientId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class RoleAppMap
    {
        [Key]
        [Column(Order = 1)]
        public Guid RoleID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int AppID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class MailBodyConfiguration
    {
        [Key]
        public int ID { get; set; }
        public string MailType { get; set; }
        public string MailBody { get; set; }
        public string MailSubject { get; set; }
    }

    public class NewsAndNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NID { get; set; }
        public string? NTital { get; set; }
        public DateTime? NDate { get; set; }
        public string? NDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class UserLoginHistory
    {
        [Key]
        public int ID { get; set; }
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }

    public class AuthTokenHistory
    {
        [Key]
        public int TokenHistoryID { get; set; }
        public Guid UserID { get; set; }
        public string Token { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpireOn { get; set; }
        public DateTime? UsedOn { get; set; }
        public bool IsUsed { get; set; }
        public string? Comment { get; set; }
    }

    public class EmailConfiguration
    {
        [Key]
        public int ID { get; set; }
        public string ServerAddress { get; set; }
        public string MailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IncomingIMAPPort { get; set; }
        public string IncomingPOP3Port { get; set; }
        public string OutgoingPort { get; set; }
        public bool IsSSL { get; set; }
        public bool IsActive { get; set; }
    }

    [Table("otpConfiguration")]
    public class otpConfiguration
    {
        [Key]
        public int ID { get; set; }
        public string? method { get; set; }
        public string? send_to { get; set; }
        public string? msg { get; set; }
        public string? msg_type { get; set; }
        public string? userid { get; set; }
        public string? auth_scheme { get; set; }
        public string? password { get; set; }
        public string? v { get; set; }
        public string? format { get; set; }
    }

    public class PasswordResetOtpHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string OTP { get; set; }
        public bool OTPIsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime ExpiryOn { get; set; }
    }

    public class DocumentMaster
    {
        [Key]
        public int Id { get; set; }
        public string? DocumentId { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentExtention { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentPath { get; set; }
        public string? DocumentURL { get; set; }
        public int? DocumentNameId { get; set; }
        public string? DocumentXeroxOrOriginal { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public int? InitiationId { get; set; } = 0;
        public bool? DocumentReadStatus { get; set; } = false;
    }

    public class DepartmentMaster
    {
        [Key]
        public int Id { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class ManagerUserMap
    {
        [Key]
        public int MapID { get; set; }
        public Guid ManagerID { get; set; }
        public Guid UserID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}