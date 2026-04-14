using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Repositories;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ticket>> GetAvailableTicketsByEventIdAsync(int eventId)
    {
        return await _dbSet
            .Where(t => t.EventId == eventId && t.Status == TicketStatus.Available)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByEventIdAsync(int eventId)
    {
        return await _dbSet
            .Where(t => t.EventId == eventId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByBookingIdAsync(int bookingId)
    {
        return await _dbSet
            .Where(t => t.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<int> GetAvailableTicketCountAsync(int eventId)
    {
        // Обновляем статусы прошедших билетов перед подсчетом
        await UpdateExpiredTicketsAsync();
        return await _dbSet
            .CountAsync(t => t.EventId == eventId && t.Status == TicketStatus.Available);
    }

    public async Task<IEnumerable<Ticket>> ReserveTicketsAsync(int eventId, int count)
    {
        // Обновляем статусы прошедших билетов перед резервированием
        await UpdateExpiredTicketsAsync();
        var tickets = await _dbSet
            .Where(t => t.EventId == eventId && t.Status == TicketStatus.Available)
            .Take(count)
            .ToListAsync();

        foreach (var ticket in tickets)
        {
            ticket.Status = TicketStatus.Booked;
        }

        await _context.SaveChangesAsync();
        return tickets;
    }

    public async Task<int> UpdateExpiredTicketsAsync()
    {
        var now = DateTime.Now;
        
        // Находим все билеты мероприятий, которые уже прошли
        var expiredTickets = await _dbSet
            .Include(t => t.Event)
            .Where(t => t.Status != TicketStatus.Expired && 
                       (t.Event.Date.Date < now.Date || 
                        (t.Event.Date.Date == now.Date && t.Event.Time < now.TimeOfDay)))
            .ToListAsync();

        var count = 0;
        foreach (var ticket in expiredTickets)
        {
            ticket.Status = TicketStatus.Expired;
            count++;
        }

        if (count > 0)
        {
            await _context.SaveChangesAsync();
        }

        return count;
    }
}


