namespace ToyosiAndGIG.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Position { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public DateTime DateOfEmployment { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
    }
}
