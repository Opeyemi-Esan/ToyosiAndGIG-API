using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Application.Dtos
{
    public class UpdateBookingDto
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCity { get; set; }
        public string EventType { get; set; }
        public DateTime? EventDate { get; set; }
        public string Venue { get; set; }
        public string Notes { get; set; }
        public IFormFile EventFlyer { get; set; } 
        public BookingStatus Status { get; set; }
    }
}
