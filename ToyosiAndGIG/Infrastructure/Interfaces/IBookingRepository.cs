using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.Infrastructure.Interface
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllBookingAsync();
        Task<Booking> GetBookingByBookingNumberAsync(string bookingNumber);
        Task<Booking> CreateBooking(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking newBooking);
        Task<bool> DelelteBookingAsync(string bookingNumber, string deletedBy);
    }
}
