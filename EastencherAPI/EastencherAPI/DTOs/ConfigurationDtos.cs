using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApplication.DTOs
{
    public class STMPDetails
    {
        public string Host { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }

    public class EmailConfigurations
    {
        public int ID { get; set; }
        public string MailID { get; set; }
        public string MailPassword { get; set; }
        public string UserName { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public bool IsSSL { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string? DepartmentName { get; set; }
        public string? UserId { get; set; }
    }

    public class SignUpUserRequest
    {
        public string UserId { get; set; }
        public string ModifiedBy { get; set; }
    }
}