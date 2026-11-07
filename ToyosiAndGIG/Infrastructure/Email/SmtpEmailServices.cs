//using System.Net;
//using System.Net.Mail;
//using ToyosiAndGIG.Application.Interfaces;

//namespace ToyosiAndGIG.Infrastructure.Email
//{
//    public class SmtpEmailServices: IEmailServices
//    {
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<SmtpEmailServices> _logger;

//        public SmtpEmailServices(IConfiguration configuration, ILogger<SmtpEmailServices> logger)
//        {
//            _configuration = configuration;
//            _logger = logger;
//        }

//        public Task SendEmailAsync(string toEmail, string subject, string htmlBody)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task SendOtpEmailAsync(string toEmail, string otp, string purpose)
//        {
//            var smtpSettings = _configuration.GetSection("SmtpSettings");

//            using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"] ?? "587"))
//            {
//                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
//                EnableSsl = true
//            };

//            var message = new MailMessage
//            {
//                From = new MailAddress(smtpSettings["FromAddress"] ?? "no-reply@toyosiandgig.com", "ToyosiAndGIG"),
//                Subject = purpose == "EmailConfirmation" ? "Confirm your email" : "Password reset code",
//                Body = $"Your OTP code is: {otp}\n\nThis code expires in 10 minutes. If you didn't request this, ignore this email.",
//                IsBodyHtml = false
//            };
//            message.To.Add(toEmail);

//            try
//            {
//                await client.SendMailAsync(message);
//                _logger.LogInformation("OTP email sent to {Email} for {Purpose}", toEmail, purpose);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to send OTP email to {Email}", toEmail);
//                throw;
//            }
//        }
//    }
//}
