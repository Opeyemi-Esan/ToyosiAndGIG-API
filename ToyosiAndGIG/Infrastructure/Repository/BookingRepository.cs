using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Infrastructure.Data;
using ToyosiAndGIG.Infrastructure.Interface;

namespace ToyosiAndGIG.Infrastructure.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DelelteBookingAsync(string bookingNumber, string deletedBy)
        {
            var bookings = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingReference == bookingNumber && b.IsDeleted == false);
            if (bookings == null) return false;

            bookings.IsDeleted = true;
            bookings.DeletedAt = DateTime.UtcNow;
            bookings.DeletedBy = deletedBy;
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<Booking>> GetAllBookingAsync()
            => _context.Bookings
                .AsNoTracking()
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        public Task<Booking> GetBookingByBookingNumberAsync(string bookingNumber)
            => _context.Bookings.FirstOrDefaultAsync(b => b.BookingReference == bookingNumber && b.IsDeleted == false);

        public async Task<Booking> UpdateBookingAsync(Booking newBooking)
        {
            var book = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingReference == newBooking.BookingReference && b.IsDeleted == false);
            if (book == null) return null;
            await _context.SaveChangesAsync();
            return book;
        }
    }
}
