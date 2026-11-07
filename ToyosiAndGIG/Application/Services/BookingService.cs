using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Net;
using ToyosiAndGIG.Application.Dtos;
using ToyosiAndGIG.Application.Interface;
using ToyosiAndGIG.Application.Interfaces;
using ToyosiAndGIG.Domain.Entities;
using ToyosiAndGIG.Domain.Response;
using ToyosiAndGIG.Infrastructure.Data;
using ToyosiAndGIG.Infrastructure.Email;
using ToyosiAndGIG.Infrastructure.Interface;
using ToyosiAndGIG.Infrastructure.Storage;

namespace ToyosiAndGIG.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IImageCompressionService _imageCompressionService;
        private readonly ILogger<BookingService> _logger;
        private readonly IBookingRepository _bookingRepository;
        private readonly IEmailServices _emailServices;

        public BookingService(
            IBookingRepository bookingRepository,
            IFileStorageService fileStorageService,
            IImageCompressionService imageCompressionService,
            ILogger<BookingService> logger,
            IEmailServices emailServices)
        {
            _bookingRepository = bookingRepository;
            _fileStorageService = fileStorageService;
            _imageCompressionService = imageCompressionService;
            _logger = logger;
            _emailServices = emailServices;
        }

        public async Task<Response<Booking>> CreateBookingAsync(CreateBookingDto dto, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                return Response<Booking>.Fail("Invalid customer name", HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(dto.CustomerPhone))
            {
                return Response<Booking>.Fail("Invalid customer phone number", HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
            {
                return Response<Booking>.Fail("Invalid customer email", HttpStatusCode.BadRequest);
            }

            try
            {
                string flyerUrl = null;

                if (dto.EventFlyer != null)
                {
                    if (dto.EventFlyer.Length > 10 * 1024 * 1024) // 10MB cap before compression
                        return Response<Booking>.Fail("Event flyer must be under 10MB.", HttpStatusCode.BadRequest);

                    try
                    {
                        await using var fileStream = dto.EventFlyer.OpenReadStream();
                        var (compressedStream, contentType, extension) = await _imageCompressionService.CompressAsync(fileStream, dto.EventFlyer.FileName);

                        await using (compressedStream)
                        {
                            flyerUrl = await _fileStorageService.UploadAsync(
                                compressedStream,
                                $"flyer{extension}",
                                contentType,
                                StorageFolders.BookingFlyers);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogWarning(ex, "Flyer upload failed for booking by {CreatedBy}", createdBy);
                        return Response<Booking>.Fail(ex.Message, HttpStatusCode.BadRequest);
                    }
                }

                var booking = new Booking
                {
                    BookingReference = GenerateBookingNumber(),
                    CustomerName = dto.CustomerName,
                    CustomerPhone = dto.CustomerPhone,
                    CustomerEmail = dto.CustomerEmail,
                    CustomerCity = dto.CustomerCity,
                    EventType = dto.EventType,
                    EventDate = dto.EventDate,
                    Venue = dto.Venue,
                    Notes = dto.Notes,
                    EventFlyer = flyerUrl,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy 
                };

                await _bookingRepository.CreateBooking(booking);

                if (!string.IsNullOrWhiteSpace(booking.CustomerEmail))
                {
                    var subject = $"Booking Confirmation — {booking.BookingReference}";
                    var body = EmailTemplates.BookingConfirmation(booking.CustomerName, booking.BookingReference, booking.EventType, booking.EventDate);
                    await _emailServices.SendEmailAsync(booking.CustomerEmail, subject, body);
                }

                _logger.LogInformation("Booking {BookingNumber} created by {CreatedBy}", booking.BookingReference, createdBy);
                return Response<Booking>.Success(booking, "Booking submitted successfully.", HttpStatusCode.Created);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An Error occurred while creating booking");
                return Response<Booking>.Fail("Something went wrong", HttpStatusCode.InternalServerError);
            }
        }

        private static string GenerateBookingNumber()
        {
           return $"TY{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}GIG";
        }


        public async Task<Response<List<Booking>>> GetAllBookingAsync()
        {
            try
            {
                var books = await _bookingRepository.GetAllBookingAsync();

                if (books == null)
                {
                    return Response<List<Booking>>.Fail("No Booking found. Create a booking", HttpStatusCode.NotFound);
                }

                return Response<List<Booking>>.Success(books);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching all booking");
                return Response<List<Booking>>.Fail("Something went wrong", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<Booking>> GetBookingByBookingNumberAsync(string bookingNumber)
        {
            try
            {
                var book = await _bookingRepository.GetBookingByBookingNumberAsync(bookingNumber);

                if (book == null)
                {
                    return Response<Booking>.Fail($"Booking {bookingNumber} does not exist", HttpStatusCode.NotFound);
                }

                return Response<Booking>.Success(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching all booking");
                return Response<Booking>.Fail("Something went wrong", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Response<Booking>> UpdateBookingAsync(UpdateBookingDto update, string updatedBy, string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(update.CustomerName))
            {
                return Response<Booking>.Fail("Invalid customer name", HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(update.CustomerPhone))
            {
                return Response<Booking>.Fail("Invalid customer phone number", HttpStatusCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(update.CustomerEmail))
            {
                return Response<Booking>.Fail("Invalid customer email", HttpStatusCode.BadRequest);
            }

            try
            {
                var existingBooking = await _bookingRepository.GetBookingByBookingNumberAsync(bookingNumber);
                if (existingBooking == null)
                {
                    _logger.LogWarning("Booking {BookingNumber} not found for update", bookingNumber);
                    return Response<Booking>.Fail("Booking not found", HttpStatusCode.NotFound);
                }

                string newFlyerUrl = null;

                if (update.EventFlyer != null)
                {
                    if (update.EventFlyer.Length > 10 * 1024 * 1024) // 10MB cap before compression
                        return Response<Booking>.Fail("Event flyer must be under 10MB.", HttpStatusCode.BadRequest);

                    try
                    {
                        await using var fileStream = update.EventFlyer.OpenReadStream();
                        var (compressedStream, contentType, extension) = await _imageCompressionService.CompressAsync(fileStream, update.EventFlyer.FileName);

                        await using (compressedStream)
                        {
                            newFlyerUrl = await _fileStorageService.UploadAsync(
                                compressedStream,
                                $"flyer{extension}",
                                contentType,
                                StorageFolders.BookingFlyers);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogWarning(ex, "Flyer upload failed for booking {BookingNumber} by {UpdatedBy}", bookingNumber, updatedBy);
                        return Response<Booking>.Fail(ex.Message, HttpStatusCode.BadRequest);
                    }

                    // Clean up the old flyer only after the new one uploads successfully —
                    // avoids leaving the booking with no flyer at all if cleanup ran first
                    // and the new upload then failed.
                    if (!string.IsNullOrWhiteSpace(existingBooking.EventFlyer))
                    {
                        await _fileStorageService.DeleteAsync(existingBooking.EventFlyer);
                    }
                }

                existingBooking.CustomerName = update.CustomerName ?? existingBooking.CustomerName;
                existingBooking.CustomerPhone = update.CustomerPhone ?? existingBooking.CustomerPhone;
                existingBooking.CustomerEmail = update.CustomerEmail ?? existingBooking.CustomerEmail;
                existingBooking.CustomerCity = update.CustomerCity ?? existingBooking.CustomerCity;
                existingBooking.EventType = update.EventType ?? existingBooking.EventType;
                existingBooking.EventDate = update.EventDate ?? existingBooking.EventDate;
                existingBooking.Venue = update.Venue ?? existingBooking.Venue;
                existingBooking.Notes = update.Notes ?? existingBooking.Notes;
                //existingBooking.Status = update.Status;
                existingBooking.EventFlyer = newFlyerUrl ?? existingBooking.EventFlyer;
                existingBooking.LastUpdatedAt = DateTime.UtcNow;
                existingBooking.LastUpdatedBy = updatedBy;

                await _bookingRepository.UpdateBookingAsync(existingBooking);

                _logger.LogInformation("Booking {BookingNumber} updated by {UpdatedBy}", existingBooking.BookingReference, updatedBy);
                return Response<Booking>.Success(existingBooking, "Booking updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating booking {BookingNumber}", bookingNumber);
                return Response<Booking>.Fail("Something went wrong", HttpStatusCode.InternalServerError);
            }
        }


     
        public async Task<Response<string>> UpdateStatusAsync(string bookingNumber, BookingStatus status, string updatedBy)
        {
            try
            {
                var booking = await _bookingRepository.GetBookingByBookingNumberAsync(bookingNumber);
                if (booking == null)
                    return Response<string>.Fail("Booking not found.", HttpStatusCode.NotFound);

                booking.Status = status;
                booking.LastUpdatedAt = DateTime.UtcNow;
                booking.LastUpdatedBy = updatedBy;

                await _bookingRepository.UpdateBookingAsync(booking);

                if (!string.IsNullOrWhiteSpace(booking.CustomerEmail))
                {
                    var (subject, body) = status switch
                    {
                        BookingStatus.Approved => (
                            $"Your booking is confirmed! — {booking.BookingReference}",
                            EmailTemplates.BookingApproved(booking.CustomerName, booking.BookingReference, booking.EventType, booking.EventDate, booking.Venue)
                        ),
                        BookingStatus.Rejected => (
                            $"Update on your booking — {booking.BookingReference}",
                            EmailTemplates.BookingRejected(booking.CustomerName, booking.BookingReference, booking.EventType)
                        ),
                        _ => (null, null) // Pending or any other status — no email needed
                    };

                    if (subject != null)
                    {
                        await _emailServices.SendEmailAsync(booking.CustomerEmail, subject, body);
                    }
                }

                _logger.LogInformation("Booking {BookingNumber} status changed to {Status} by {UpdatedBy}", booking.BookingReference, status, updatedBy);
                return Response<string>.Success(null, $"Booking status updated to {status}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking status for {BookingNumber}", bookingNumber);
                return Response<string>.Fail("Something went wrong", HttpStatusCode.InternalServerError);
            }
        }


        public async Task<Response<string>> DelelteBookingAsync(string bookingNumber, string deletedBy)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber))
            {
               return Response<string>.Fail("Invalid booking number", HttpStatusCode.BadRequest);
            }
            try
            {
                var book = await _bookingRepository.DelelteBookingAsync(bookingNumber, deletedBy);

                if (!book)
                {
                    _logger.LogWarning($"Booking {bookingNumber} not found for deletion");
                    return Response<string>.Fail("Booking not found.", HttpStatusCode.NotFound);
                }

                
                return Response<string>.Success($"Booking {bookingNumber} deleted successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occured while deleting booking");
                return Response<string>.Fail("Something went wrong", HttpStatusCode.InternalServerError);
            }
        }
       
    }
}
