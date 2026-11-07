namespace ToyosiAndGIG.Application.Interfaces
{
    public interface IEmailServices
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendOtpEmailAsync(string toEmail, string otp, string purpose);
    }
}
