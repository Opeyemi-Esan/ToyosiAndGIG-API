namespace ToyosiAndGIG.Application.Dtos
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // e.g. "Member", "Manager"
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime DateOfEmployment { get; set; }
    }
}
