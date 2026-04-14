using EastencherAPI.DBContext;
using AuthApplication.Models;
using EastencherAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
//using AuthApplication.Helpers; 

//using Org.BouncyCastle.Ocsp;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
//using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.WebRequestMethods;

namespace AuthApplication.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public EmailService(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }


        public async Task<bool> SendUserCreatedMailForUser(string FullName, string toEmail, string Password, string url)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration.FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);
                var ebody = _dbContext.MailBodyConfigurations.FirstOrDefault(k => k.ID == 1);
                if (ec != null && ebody != null)
                {
                    MailMessage message = new MailMessage();
                    using (SmtpClient smtp = new SmtpClient(ec.ServerAddress))
                    {
                        message.From = new MailAddress(ec.MailAddress);
                        message.To.Add(new MailAddress(toEmail));
                        message.Subject = ebody.MailSubject;
                        message.IsBodyHtml = false;
                        message.Body = ebody.MailBody;
                        message.Body = message.Body.Replace("@LoginLink@", url);
                        message.Body = message.Body.Replace("@DearUser@", FullName);
                        message.Body = message.Body.Replace("@UserName@", toEmail);
                        message.Body = message.Body.Replace("@Password@", Password);
                        smtp.Port = int.Parse(ec.OutgoingPort);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 60000;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(ec.UserName, ec.Password);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        message.BodyEncoding = UTF8Encoding.UTF8;
                        message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        message.IsBodyHtml = true;
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                        await smtp.SendMailAsync(message);

                        Log.DataLog(toEmail, $"User credentials has been shared successfully to the mail {toEmail}", "Email Log");
                    }
                }
                else
                {
                    throw new Exception("Email configuration or Invoice details not found");
                }

                return true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Log.Error(toEmail, $"UserCreatedMailForUser/SendMail/MailboxBusy/MailboxUnavailable/SmtpFailedRecipientsException:Inner {ex.Message}", "Email Log");
                    }
                    else
                    {
                        Log.Error(toEmail, $"UserCreatedMailForUser/SendMail/SmtpFailedRecipientsException:Inner - {ex.Message}", "Email Log");
                    }
                }
                Log.Error(toEmail, $"UserCreatedMailForUser/SendMail/SmtpFailedRecipientsException:- {ex.Message}", "Email Log");
                return false;
            }
            catch (SmtpException ex)
            {
                Log.Error(toEmail, $"UserCreatedMailForUser/SendMail/SmtpException:- {ex.Message}", "Email Log");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(toEmail, $"UserCreatedMailForUser/SendMail/Exception:- {ex.Message}", "Email Log");
                return false;
            }
        }


        public async Task<bool> SendMailForUserResetPasswordMail(string code, string UserName, string toEmail, string userID, string siteURL)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration.FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);
                var ebody = _dbContext.MailBodyConfigurations.FirstOrDefault(k => k.ID == 2);
                if (ec != null && ebody != null)
                {
                    MailMessage message = new MailMessage();
                    using (SmtpClient smtp = new SmtpClient(ec.ServerAddress))
                    {
                        message.From = new MailAddress(ec.MailAddress);
                        message.To.Add(new MailAddress(toEmail));
                        message.Subject = ebody.MailSubject;
                        message.IsBodyHtml = false;
                        message.Body = ebody.MailBody;
                        message.Body = message.Body.Replace("@ResetLink@", siteURL + "?token=" + code + "&Id=" + userID);
                        message.Body = message.Body.Replace("@UserName@", UserName);
                        smtp.Port = int.Parse(ec.OutgoingPort);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 60000;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(ec.UserName, ec.Password);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        message.BodyEncoding = UTF8Encoding.UTF8;
                        message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        message.IsBodyHtml = true;
                        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                        await smtp.SendMailAsync(message);
                        Log.DataLog(toEmail, $"Password Reset link has been sent successfully to the user email {toEmail}", "Password Reset mail Log");
                    }
                }
                else
                {
                    throw new Exception("Email configuration or Invoice details not found");
                }
                return true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                    {

                        Log.Error(toEmail, $"UserResetPassword/SendMail/MailboxBusy/MailboxUnavailable/SmtpFailedRecipientsException:Inner- {ex.Message}",
                         "Password Reset mail Log");
                    }
                    else
                    {
                        Log.Error(toEmail, $"UserResetPassword/SendMail/SmtpFailedRecipientsException:Inner- {ex.Message}",
                      "Password Reset mail Log");
                    }
                }
                Log.Error(toEmail, $"UserResetPassword/SendMail/SmtpFailedRecipientsException:- {ex.Message}",
                    "Password Reset mail Log");
                return false;
            }
            catch (SmtpException ex)
            {
                Log.Error(toEmail, $"UserResetPassword/SendMail/SmtpException {ex.Message}",
                   "Password Reset mail Log");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(toEmail, $"UserResetPassword/SendMail/Exception {ex.Message}",
                  "Password Reset mail Log");
                return false;
            }
        }


        public async Task<string> GenerateAndSendOTP(string email)
        {
            // Generate the OTP using your desired logic
            Random random = new Random();
            string generatedOtp = random.Next(100000, 999999).ToString();
            if (!string.IsNullOrEmpty(generatedOtp))
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    DateTime expiryTime = DateTime.Now.AddMinutes(10);
                    // Save OTP details to the database
                    var passwordReset = new PasswordResetOtpHistory
                    {
                        Email = email,
                        OTP = generatedOtp,
                        OTPIsActive = true,
                        CreatedOn = DateTime.Now,
                        ExpiryOn = expiryTime,
                        CreatedBy = user.Email
                    };
                    _dbContext.PasswordResetOtpHistorys.Add(passwordReset);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Send OTP via email
            try
            {
                var ec = _dbContext.EmailConfiguration.FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);

                if (ec != null)
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient(ec.ServerAddress);

                    message.From = new MailAddress(ec.MailAddress);
                    message.To.Add(new MailAddress(email));
                    message.Subject = "Password Reset OTP";
                    message.IsBodyHtml = false;
                    string messageBody = $"Your OTP is: {generatedOtp}. Please use it within 5 minutes.";
                    message.Body = messageBody;


                    smtp.Port = int.Parse(ec.OutgoingPort);
                    smtp.EnableSsl = true;
                    smtp.Timeout = 60000;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(ec.MailAddress, ec.Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await smtp.SendMailAsync(message);
                    Log.DataLog(email, $"OTP {generatedOtp} has been sent to the usere email {email} to reset the password", "OTP Log");
                    //WriteLog.AddMailWriteLog($"Sent email to user {{0}} successfully\" to {email}", email);
                    return generatedOtp;
                }
                else
                {
                    throw new Exception("Email configuration not found");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions when sending email
                //ErrorLog.AddMailErrorLog("GenrateOtpSent", ex.Message);
                Log.Error(email, $"While sending or generating otp to send the mail for the user email {email} following error occured", "OTP Log");
                throw new Exception("Failed to send OTP via email", ex);
            }
        }


        public async Task<bool> SendActivationMail(string email, string userName, bool active)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration.FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);
                if (ec == null)
                    throw new Exception("Email configuration not found.");

                string statusText = active ? "Activated" : "Deactivated";
                string subject = $"User Account {statusText} - AuthApplication";

                StringBuilder sb = new StringBuilder();
                sb.Append($@"
                            <div style='padding:20px; font-family:Segoe UI;'>
                                <div style='border:1px solid #dbdbdb; padding:20px; background-color:#f9f9f9;'>
                                    <h2 align='center' style='color:#2a2a2a;'>Auth Application</h2>
                                    <p>Dear {userName},</p>
                                    <p>Your account has been <b>{statusText}</b> by the administrator.</p>
                         ");

                if (active)
                {
                    sb.Append($@"
                                <p>You can now log in to the application using your credentials.</p>
                                <p><a href='https://your-login-url.com' target='_blank' style='background:#0078d7;color:#fff;padding:10px 15px;text-decoration:none;border-radius:5px;'>Login Now</a></p>
                             ");
                }
                else
                {
                    sb.Append($@"
                                <p>Your access to the application has been temporarily disabled. Please contact support if this was unexpected.</p>
                             ");
                }

                sb.Append(@"
                            <br/>
                            <p>Regards,<br/>Admin Team</p>
                            <hr style='border:none;border-top:1px solid #ddd;'/>
                            <p align='center' style='font-size:10px;color:#666;'>
                                Sensitivity: Internal & Restricted<br/>
                                This email and any attachments are confidential. If you are not the intended recipient, please delete it immediately.
                            </p>
                            </div>
                            </div>
                            ");

                // Configure SMTP client
                using (SmtpClient client = new SmtpClient(ec.ServerAddress))
                {
                    client.Port = int.Parse(ec.OutgoingPort);
                    client.EnableSsl = true;
                    client.Timeout = 60000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(ec.MailAddress, ec.Password);

                    // Build the mail
                    MailMessage mail = new MailMessage(ec.MailAddress, email, subject, sb.ToString())
                    {
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };

                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    // Send email
                    await client.SendMailAsync(mail);
                }

                // Log success
                Log.DataLog(email, $"Activation mail sent successfully to {email} ({statusText})", "User Activation Mail Log");

                return true;
            }
            catch (SmtpFailedRecipientsException ex)
            {
                Log.Error(email, $"SendActivationMail/SmtpFailedRecipientsException: {ex.Message}", "User Activation Mail Log");
                return false;
            }
            catch (SmtpException ex)
            {
                Log.Error(email, $"SendActivationMail/SmtpException: {ex.Message}", "User Activation Mail Log");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(email, $"SendActivationMail/Exception: {ex.Message}", "User Activation Mail Log");
                return false;
            }
        }


        public async Task<bool> SendPasswordChangeMail(string email, string userName)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration.FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);
                if (ec == null)
                    throw new Exception("Email configuration not found.");

                string subject = $"Your Password Has Been Changed - AuthApplication";

                var sb = new StringBuilder();
                sb.Append($@"
                        <div style='padding:20px; font-family:Segoe UI;'>
                        <h2 align='center' style='color:#2a2a2a;'>Password Change Notification</h2>
                        <p>Dear {userName},</p>
                        <p>This is a security notification to inform you that your account password has been successfully changed.</p>
                        <p>If you did not change your password, please reset it immediately and contact our support team.</p>
                        <br/>
                        <p>Regards,<br/>Admin</p>
                        <hr style='border:none;border-top:1px solid #ddd;'/>
                        <p align='center' style='font-size:10px;color:#666;'>
                                    Sensitivity: Internal & Restricted<br/>
                                    This email and any attachments are confidential. If you are not the intended recipient, please delete it immediately.
                        </p>
                        </div>");

                using (var client = new SmtpClient(ec.ServerAddress))
                {
                    client.Port = int.Parse(ec.OutgoingPort);
                    client.EnableSsl = true;
                    client.Timeout = 60000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(ec.MailAddress, ec.Password);

                    var mail = new MailMessage(ec.MailAddress, email, subject, sb.ToString())
                    {
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };

                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    await client.SendMailAsync(mail);
                }

                Log.DataLog(email, $"Password change mail sent successfully to {email}", "Password Change Mail Log");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(email, $"SendPasswordChangeMail/Exception: {ex.Message}", "Password Change Mail Log");
                return false;
            }
        }
        //send if any field changed 
        public async Task<bool> SendUserUpdateMail(User oldUser, User newUser)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration.FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);
                if (ec == null)
                    throw new Exception("Email configuration not found.");

                var changes = new List<(string Field, string OldValue, string NewValue)>();

                void AddChange(string fieldName, string oldValue, string newValue)
                {
                    if ((oldValue ?? "") != (newValue ?? ""))
                        changes.Add((fieldName, oldValue ?? "", newValue ?? ""));
                }

                AddChange("First Name", oldUser.FullName, newUser.FullName);
                AddChange("Email", oldUser.Email, newUser.Email);
                AddChange("User Name", oldUser.UserName, newUser.UserName);
                AddChange("Contact Number", oldUser.PhoneNumber, newUser.PhoneNumber);
                AddChange("Client ID", oldUser.ClientId, newUser.ClientId);
                AddChange("Date of Birth", oldUser.RoleName, newUser.RoleName);
                //AddChange("Role Name", oldUser.RoleID, newUser.RoleID);


                if (!changes.Any())
                    return false; // No changes, do not send mail.

                string subject = $"Your Profile Has Been Updated - AuthApplication";

                var sb = new StringBuilder();
                sb.Append($@"
                            <div style='padding:20px; font-family:Segoe UI;'>
                            <h2 align='center' style='color:#2a2a2a;'>Profile Updated</h2>
                            <p>Dear {newUser.FullName ?? newUser.UserName},</p>
                            <p>Your profile was updated with the following changes:</p>
                            <table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse;'>
                            <tr>
                            <th>Field</th>
                            <th>Existing Data</th>
                            <th>New Data</th>
                            </tr>");
                foreach (var change in changes)
                {
                    sb.Append($@"
                                <tr>
                                <td>{change.Field}</td>
                                <td>{System.Net.WebUtility.HtmlEncode(change.OldValue)}</td>
                                <td>{System.Net.WebUtility.HtmlEncode(change.NewValue)}</td>
                                </tr>");
                }
                sb.Append(@"
                                </table>
                                <br/>
                                <p>If you did not request these changes or if you have any questions, please contact support immediately.</p>
                                <br/>
                                <p>Regards,<br/>Admin</p>
                                <hr style='border:none;border-top:1px solid #ddd;'/>
                                <p align='center' style='font-size:10px;color:#666;'>
                                            Sensitivity: Internal & Restricted<br/>
                                            This email and any attachments are confidential. If you are not the intended recipient, please delete it immediately.
                                </p>
                                </div>
                                ");

                using (var client = new SmtpClient(ec.ServerAddress))
                {
                    client.Port = int.Parse(ec.OutgoingPort);
                    client.EnableSsl = true;
                    client.Timeout = 60000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(ec.MailAddress, ec.Password);

                    var mail = new MailMessage(ec.MailAddress, newUser.Email, subject, sb.ToString())
                    {
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };

                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    await client.SendMailAsync(mail);
                }

                Log.DataLog(newUser.Email, $"Update mail sent successfully to {newUser.Email}", "User Update Mail Log");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(newUser.Email, $"SendUserUpdateMail/Exception: {ex.Message}", "User Update Mail Log");
                return false;
            }
        }

        // Send Message content to the Email
        public async Task<bool> SendAdminMessageEmail(string email, string userName, string? message, string? subject = null)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration
                    .FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);

                if (ec == null)
                    throw new Exception("Email configuration not found.");
                string emailSubject = string.IsNullOrWhiteSpace(subject) ? "Message from Admin" : subject;

                var sb = new StringBuilder();
                sb.Append($@"
                            <div style='padding:20px;font-family:Segoe UI, Tahoma, Geneva, Verdana, sans-serif; background:#ffffff;'>
                                <h2 align='center' style='color:#2a2a2a;'>Message from Admin</h2>
                                <p>Dear <b>{userName}</b>,</p>
                                <p>{(string.IsNullOrWhiteSpace(message) ? "No message content provided." : message)}</p>
                                <br/>
                                <p>
                                    Regards,<br/>
                                    <b>Learning Admin Team</b>
                                </p>
                                <hr style='border:none;border-top:1px solid #ddd;'/>
                                <p style='font-size:10px;color:#666;text-align:center;'>
                                    Sensitivity: Internal & Restricted<br/>
                                    This email and any attachments are confidential.<br/>
                                    If you are not the intended recipient, please delete it immediately.
                                </p>
                            </div>");

                using (var client = new SmtpClient(ec.ServerAddress))
                {
                    client.Port = int.Parse(ec.OutgoingPort);
                    client.EnableSsl = true;
                    client.Timeout = 60000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(ec.MailAddress, ec.Password);

                    var mail = new MailMessage(
                        ec.MailAddress,
                        email,
                        emailSubject,
                        sb.ToString())
                    {
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true,
                        DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    };

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;

                    await client.SendMailAsync(mail);
                }

                Log.DataLog(
                    email,
                    $"Admin message email sent successfully to {email}",
                    "Admin Message Mail Log");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(
                    email,
                    $"SendAdminMessageEmail/Exception: {ex.Message}",
                    "Admin Message Mail Log");

                return false;
            }
        }

        // Send Payment Remainder Email(message)
        public async Task<bool> SendPaymentReminderMail(string email,string userName,string? customMessage = null)
        {
            try
            {
                var ec = _dbContext.EmailConfiguration
                    .FirstOrDefault(k => k.IsActive && !k.IsSSL && k.ID == 1);

                if (ec == null)
                    throw new Exception("Email configuration not found.");

                string subject = "Payment Reminder – Action Required";

                var sb = new StringBuilder();
                sb.Append($@"
                <div style='padding:20px;font-family:Segoe UI;background:#ffffff;'>
                    <h2 align='center' style='color:#2a2a2a;'>Payment Reminder</h2>
                    <p>Dear <b>{userName}</b>,</p>
                    <p>
                        {customMessage ??
                            "This is a friendly reminder regarding your pending payment. Please ensure it is completed at your earliest convenience."}
                    </p
                    <p>
                        If you have already made the payment, please ignore this message.
                        Otherwise, kindly arrange payment to avoid service disruption.
                    </p>
                    <br/>
                    <p>
                        Regards,<br/>
                        <b>Learning Admin Team</b>
                    </p>
                    <hr style='border:none;border-top:1px solid #ddd;'/>
                    <p style='font-size:10px;color:#666;text-align:center;'>
                        Sensitivity: Internal & Restricted<br/>
                        This email and any attachments are confidential.
                        If you are not the intended recipient, please delete it immediately.
                    </p>
                </div>");

                using (var client = new SmtpClient(ec.ServerAddress))
                {
                    client.Port = int.Parse(ec.OutgoingPort);
                    client.EnableSsl = true;
                    client.Timeout = 60000;
                    client.UseDefaultCredentials = false;
                    client.Credentials =
                        new NetworkCredential(ec.MailAddress, ec.Password);

                    var mail = new MailMessage(
                        ec.MailAddress,
                        email,
                        subject,
                        sb.ToString())
                    {
                        BodyEncoding = Encoding.UTF8,
                        IsBodyHtml = true,
                        DeliveryNotificationOptions =
                            DeliveryNotificationOptions.OnFailure
                    };

                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;

                    await client.SendMailAsync(mail);
                }

                Log.DataLog(
                    email,
                    $"Payment reminder mail sent successfully to {email}",
                    "Payment Reminder Mail Log");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(
                    email,
                    $"SendPaymentReminderMail/Exception: {ex.Message}",
                    "Payment Reminder Mail Log");

                return false;
            }
        }

    }
}   