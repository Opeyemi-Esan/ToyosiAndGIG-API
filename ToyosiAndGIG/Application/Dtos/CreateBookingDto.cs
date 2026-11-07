using System.ComponentModel.DataAnnotations;

namespace ToyosiAndGIG.Application.Dtos
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Customer phone is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Customer email is required.")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "Customer city is required.")]
        public string CustomerCity { get; set; }
        public string EventType { get; set; }
        public DateTime? EventDate { get; set; }
        public string Venue { get; set; }
        public string Notes { get; set; }
        public IFormFile EventFlyer { get; set; } // nullable — flyer upload is optional
    }
}
