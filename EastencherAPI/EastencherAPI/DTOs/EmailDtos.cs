using System.ComponentModel.DataAnnotations;

namespace AuthApplication.DTOs
{
    public class EmailModel
    {
        public string EmailAddress { get; set; }
        public string siteURL { get; set; }
    }

    public class SendPaymentReminderRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? SenderUserId { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? Message { get; set; }
    }

    public class SendMesageToEmail
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? SenderUserId { get; set; }
        public string? ReceiverUserId { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }

    public class NewsAndNotificationDTO
    {
        public int? NID { get; set; }
        public string? NTital { get; set; }
        public DateTime? NDate { get; set; }
        public string? NDescription { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}