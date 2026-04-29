using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(b => b.Event)
            .Include(b => b.Tickets)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByEventIdAsync(int eventId)
    {
        return await _dbSet
            .Include(b => b.User)
            .Where(b => b.EventId == eventId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
    {
        return await _dbSet
            .Include(b => b.Event)
            .Include(b => b.User)
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        return await _dbSet
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
            .SumAsync(b => b.TotalAmount);
    }

    public async Task<decimal> GetRevenueByEventIdAsync(int eventId)
    {
        return await _dbSet
            .Where(b => b.EventId == eventId &&
                       (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed))
            .SumAsync(b => b.TotalAmount);
    }
}


