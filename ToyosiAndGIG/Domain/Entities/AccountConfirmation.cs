namespace ToyosiAndGIG.Domain.Entities
{
    public enum OtpPurpose
    {
        EmailConfirmation = 1,
        PasswordReset = 2
    }

    public class AccountConfirmation
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string OTP { get; set; }
        public OtpPurpose Purpose { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
