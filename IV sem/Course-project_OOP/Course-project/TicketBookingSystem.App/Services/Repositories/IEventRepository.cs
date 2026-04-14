using TicketBookingSystem.Models;

namespace TicketBookingSystem.Services.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetUpcomingEventsAsync();
    Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm);
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task ReplaceEventPhotosAsync(int eventId, IEnumerable<byte[]> photoData);
    Task<Event?> GetByIdForUpdateAsync(int id);
    Task<Event?> GetByIdForUpdateWithoutPhotosAsync(int id);
    Task<Event> UpdateEventWithPhotosAsync(Event eventEntity, IEnumerable<byte[]> photoData);
}


