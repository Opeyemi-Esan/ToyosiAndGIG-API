using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interface;
using ToyosiAndGIG.Application.Services;
using ToyosiAndGIG.Domain.Entities;

namespace ToyosiAndGIG.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [HttpPost("create-booking")]
        [AllowAnonymous]
        [EnableRateLimiting("AuthPolicy")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateBooking([FromForm] CreateBookingDto dto)
        {
            var createdBy = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.Email)
                : dto.CustomerEmail;

            var response = await _bookingService.CreateBookingAsync(dto, createdBy);
            return StatusCode((int)response.StatusCode, response);
        }

        
        [HttpGet("all-bookings")]
        [Authorize(Roles = $"{Roles.Member},{Roles.Manager},{Roles.Owner}")]
        public async Task<IActionResult> GetAllBookings()
        {
            var response = await _bookingService.GetAllBookingAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("get-booking/{bookingNumber}")]
        [Authorize(Roles = $"{Roles.Member},{Roles.Manager},{Roles.Owner}")]
        public async Task<IActionResult> GetBookingByBookingNumber(string bookingNumber)
        {
            var response = await _bookingService.GetBookingByBookingNumberAsync(bookingNumber);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-booking/{bookingNumber}")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Owner}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBooking(string bookingNumber, [FromForm] UpdateBookingDto dto)
        {
            var updatedBy = User.FindFirstValue(ClaimTypes.Email);
            var response = await _bookingService.UpdateBookingAsync(dto, updatedBy, bookingNumber);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update-booking-status/{bookingNumber}")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Owner}")]
        public async Task<IActionResult> UpdateStatus(string bookingNumber, BookingStatus status)
        {
            var updatedBy = User.FindFirstValue(ClaimTypes.Email);
            var response = await _bookingService.UpdateStatusAsync(bookingNumber, status, updatedBy);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpDelete("delete-booking/{bookingNumber}")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Owner}")]
        public async Task<IActionResult> DeleteBooking(string bookingNumber)
        {
            var deletedBy = User.FindFirstValue(ClaimTypes.Email);
            var response = await _bookingService.DelelteBookingAsync(bookingNumber, deletedBy);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
