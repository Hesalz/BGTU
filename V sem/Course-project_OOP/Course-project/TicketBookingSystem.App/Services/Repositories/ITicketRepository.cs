using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Repositories;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetAvailableTicketsByEventIdAsync(int eventId);
    Task<IEnumerable<Ticket>> GetTicketsByEventIdAsync(int eventId);
    Task<IEnumerable<Ticket>> GetTicketsByBookingIdAsync(int bookingId);
    Task<int> GetAvailableTicketCountAsync(int eventId);
    Task<IEnumerable<Ticket>> ReserveTicketsAsync(int eventId, int count);
    Task<int> UpdateExpiredTicketsAsync();
}


