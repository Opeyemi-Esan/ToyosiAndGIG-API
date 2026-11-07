using System;

namespace ToyosiAndGIG.Domain.Entities
{
    public class Member
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; }
        public string Bio { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
