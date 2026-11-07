using System.ComponentModel.DataAnnotations;

namespace ToyosiAndGIG.Application.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Username { get; set; }
        [Required]
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        //[Required]
        //public string Role { get; set; }
    }
}
