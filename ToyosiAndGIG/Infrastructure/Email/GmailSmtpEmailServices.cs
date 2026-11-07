using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using ToyosiAndGIG.Application.Interfaces;

namespace ToyosiAndGIG.Infrastructure.Email
{
    public class GmailSmtpEmailServices : IEmailServices
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<GmailSmtpEmailServices> _logger;

        public GmailSmtpEmailServices(IOptions<SmtpSettings> settings, ILogger<GmailSmtpEmailServices> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.AppPassword),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromAddress, _settings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent via Gmail SMTP to {Email}: {Subject}", toEmail, subject);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Gmail SMTP failed to send email to {Email}: {Subject}", toEmail, subject);
                throw new InvalidOperationException("Failed to send email. Please try again.");
            }
        }

        public Task SendOtpEmailAsync(string toEmail, string otp, string purpose)
        {
            var subject = purpose == "EmailConfirmation" ? "Confirm your email" : "Password reset code";
            var body = EmailTemplates.OtpEmail(otp, purpose);
            return SendEmailAsync(toEmail, subject, body);
        }
    }
}
