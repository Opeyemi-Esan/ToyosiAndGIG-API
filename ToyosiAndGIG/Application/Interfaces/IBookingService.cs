using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;

namespace ToyosiAndGIG.Application.Interface
{
    public interface IBookingService
    {
        Task<Response<List<Booking>>> GetAllBookingAsync();
        Task<Response<Booking>> GetBookingByBookingNumberAsync(string bookingNumber);
        Task<Response<Booking>> CreateBookingAsync(CreateBookingDto dto, string createdBy);
        Task<Response<Booking>> UpdateBookingAsync(UpdateBookingDto update, string updatedBy, string bookingNumber);
        Task<Response<string>> UpdateStatusAsync(string bookingNumber, BookingStatus status, string updatedBy);
        Task<Response<string>> DelelteBookingAsync(string bookingNumber, string deletedBy);
    }
}
