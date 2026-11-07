namespace ToyosiAndGIG.Application.Dtos
{
    public class MemberDto
    {
        public string Name { get; set; } 
        public string Role { get; set; }
        public string Bio { get; set; }
        public IFormFile PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
