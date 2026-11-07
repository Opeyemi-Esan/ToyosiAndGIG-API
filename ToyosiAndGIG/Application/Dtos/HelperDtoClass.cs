namespace ToyosiAndGIG.Application.Dtos
{
    public class HelperDtoClass
    {
        public class ConfirmEmailDto
        {
            public string Email { get; set; }
            public string OTP { get; set; }
        }

        public class ResendOtpDto
        {
            public string Email { get; set; }
        }

        public class ForgotPasswordDto
        {
            public string Email { get; set; }
        }

        public class ResetPasswordDto
        {
            public string Email { get; set; }
            public string OTP { get; set; }
            public string NewPassword { get; set; }
        }
    }
}
