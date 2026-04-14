using System.ComponentModel.DataAnnotations;

namespace AuthApplication.DTOs
{
    public class ChangePassword
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Password must contain at least one alphabetic character and one special character.")]
        public string NewPassword { get; set; }
    }

    public class ForgotPassword
    {
        public Guid UserID { get; set; }
        public string EmailAddress { get; set; }
        public string NewPassword { get; set; }
        public string Token { get; set; }
    }

    public class ForgotPasswordTokenCheck
    {
        public string Token { get; set; }
    }

    public class PasswordChangeRequest
    {
        public string EmailorMobileNo { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }
}