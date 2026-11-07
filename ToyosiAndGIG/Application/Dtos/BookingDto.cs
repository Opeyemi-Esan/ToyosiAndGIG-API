using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Dtos
{
    public class BookingDto
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCity { get; set; }
        public string EventType { get; set; }
        public DateTime? EventDate { get; set; }
        public string Venue { get; set; }
        public string Notes { get; set; }
        public BookingStatus Status { get; set; } 
        public IFormFile EventFlyer { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
