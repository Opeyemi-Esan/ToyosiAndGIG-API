using System.ComponentModel.DataAnnotations;

namespace ToyosiAndGIG.Application.Dtos
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        // Only applied if the target account is an employee.
        // Left null/ignored for public (Customer) accounts.
        public string Position { get; set; }
        public string Department { get; set; }
    }
}
