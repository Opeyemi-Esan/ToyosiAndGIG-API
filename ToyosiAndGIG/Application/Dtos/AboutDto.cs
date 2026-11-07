namespace ToyosiAndGIG.Application.Dtos
{
    public class AboutDto
    {
        public string Biography { get; set; }
        public string Vision { get; set; }
        public string Mission { get; set; }
        public string Achievements { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
