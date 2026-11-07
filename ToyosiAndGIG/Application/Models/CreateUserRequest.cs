namespace ToyosiAndGIG.Application.Models
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; } // falls back to Email if not provided
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsAdminAction { get; set; } // true = bypass public-only role restriction
    }
}
