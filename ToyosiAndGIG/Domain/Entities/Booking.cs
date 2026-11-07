using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace ToyosiAndGIG.Domain.Entities
{
    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Booking
    {
        public int Id { get; set; } 
        public string BookingReference { get; set; }
        public string CustomerName { get; set; } 
        public string CustomerPhone { get; set; } 
        public string CustomerEmail { get; set; }
        public string CustomerCity { get; set; }
        public string EventType { get; set; }
        public DateTime? EventDate { get; set; }
        public string Venue { get; set; }
        public string Notes { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string EventFlyer { get; set; }
        public DateTime? SmsLastSent { get; set; }
        public bool IsEVentDone { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy {  get; set; } 
        public DateTime? LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
