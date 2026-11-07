using System;

namespace ToyosiAndGIG.Domain.Entities
{
    public class AboutInfo
    {
        public int Id { get; set; }
        public string Biography { get; set; } 
        public string Vision { get; set; }
        public string Mission { get; set; }
        public string Achievements { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
