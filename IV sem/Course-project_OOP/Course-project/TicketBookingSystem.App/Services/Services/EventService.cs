using TicketBookingSystem.Models;
using TicketBookingSystem.Services.Repositories;

namespace TicketBookingSystem.Services.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly IBookingRepository _bookingRepository;

    public EventService(IEventRepository eventRepository, ITicketRepository ticketRepository, IBookingRepository bookingRepository)
    {
        _eventRepository = eventRepository;
        _ticketRepository = ticketRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<Event> CreateEventAsync(Event eventEntity)
    {
        eventEntity.CreatedAt = DateTime.UtcNow;
        var photos = eventEntity.Photos?.Select(p => p.ImageData).ToList() ?? new List<byte[]>();
        eventEntity.Photos = new List<EventPhoto>();
        var createdEvent = await _eventRepository.AddAsync(eventEntity);

        // Create tickets for the event
        var tickets = new List<Ticket>();
        for (int i = 0; i < eventEntity.TotalTickets; i++)
        {
            tickets.Add(new Ticket
            {
                EventId = createdEvent.Id,
                Status = Models.Enums.TicketStatus.Available,
                CreatedAt = DateTime.UtcNow
            });
        }

        foreach (var ticket in tickets)
        {
            await _ticketRepository.AddAsync(ticket);
        }

        await _eventRepository.ReplaceEventPhotosAsync(createdEvent.Id, photos);

        return createdEvent;
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _eventRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
    {
        return await _eventRepository.GetUpcomingEventsAsync();
    }

    public async Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllEventsAsync();

        return await _eventRepository.SearchEventsAsync(searchTerm);
    }

    public async Task<Event> UpdateEventAsync(Event eventEntity)
    {
        eventEntity.UpdatedAt = DateTime.UtcNow;
        var photos = eventEntity.Photos?.Select(p => p.ImageData).ToList() ?? new List<byte[]>();
        
        // Проверяем наличие бронирований ДО обновления
        var hasBookings = await HasBookingsAsync(eventEntity.Id);
        
        // Если есть бронирования, не обновляем цену
        if (hasBookings)
        {
            // Сохраняем текущую цену из базы
            var currentEvent = await _eventRepository.GetByIdAsync(eventEntity.Id);
            if (currentEvent != null)
            {
                eventEntity.Price = currentEvent.Price;
            }
        }

        // Используем единый метод для обновления Event и Photos в одной транзакции
        // Это предотвращает конфликты и ускоряет выполнение
        var updatedEvent = await _eventRepository.UpdateEventWithPhotosAsync(eventEntity, photos);
        
        // Отсоединяем Event от контекста перед возвратом, чтобы избежать конфликтов
        // и возвращаем его напрямую, так как он уже обновлен
        return updatedEvent;
    }

    public async Task DeleteEventAsync(int id)
    {
        await _eventRepository.DeleteAsync(id);
    }

    public async Task<bool> EventExistsAsync(int id)
    {
        return await _eventRepository.ExistsAsync(id);
    }

    public async Task<bool> HasBookingsAsync(int eventId)
    {
        var bookings = await _bookingRepository.GetBookingsByEventIdAsync(eventId);
        return bookings.Any();
    }

    public async Task<Event?> GetEventByIdForEditAsync(int id)
    {
        return await _eventRepository.GetByIdForUpdateAsync(id);
    }
}


