using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Services;

public interface IBookingService
{
    Task<Booking> CreateBookingAsync(int userId, int eventId, int ticketCount);
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId);
    Task<bool> ConfirmBookingAsync(int bookingId);
    Task<bool> CancelBookingAsync(int bookingId);
    Task<bool> BookingExistsAsync(int id);
}


