using TicketBookingSystem.Models;

namespace TicketBookingSystem.Services.Services;

public interface IEventService
{
    Task<Event> CreateEventAsync(Event eventEntity);
    Task<Event?> GetEventByIdAsync(int id);
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<Event> UpdateEventAsync(Event eventEntity);
    Task DeleteEventAsync(int id);
    Task<bool> EventExistsAsync(int id);
    Task<bool> HasBookingsAsync(int eventId);
    Task<Event?> GetEventByIdForEditAsync(int id);
}


