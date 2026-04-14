using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId);
    Task<IEnumerable<Booking>> GetBookingsByEventIdAsync(int eventId);
    Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status);
    Task<decimal> GetTotalRevenueAsync();
    Task<decimal> GetRevenueByEventIdAsync(int eventId);
}


