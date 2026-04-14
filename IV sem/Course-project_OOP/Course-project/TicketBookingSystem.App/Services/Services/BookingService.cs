using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.Services.Repositories;

namespace TicketBookingSystem.Services.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ITicketRepository _ticketRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        IEventRepository eventRepository,
        ITicketRepository ticketRepository)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _ticketRepository = ticketRepository;
    }

    public async Task<Booking> CreateBookingAsync(int userId, int eventId, int ticketCount)
    {
        // Валидация количества билетов
        if (ticketCount <= 0)
        {
            if (ticketCount == 0)
            {
                throw new InvalidOperationException("Необходимо выбрать хотя бы один билет для бронирования.");
            }
            else
            {
                throw new InvalidOperationException($"Некорректное количество билетов: {ticketCount}. Количество билетов не может быть отрицательным числом. Пожалуйста, введите положительное число.");
            }
        }

        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new InvalidOperationException("Мероприятие не найдено");

        var availableCount = await _ticketRepository.GetAvailableTicketCountAsync(eventId);
        if (availableCount < ticketCount)
        {
            var availableWord = availableCount == 1 ? "билет" : availableCount < 5 ? "билета" : "билетов";
            var requestedWord = ticketCount == 1 ? "билет" : ticketCount < 5 ? "билета" : "билетов";
            throw new InvalidOperationException($"Недостаточно билетов для бронирования. Доступно: {availableCount} {availableWord}, запрошено: {ticketCount} {requestedWord}.");
        }

        // Reserve tickets
        var tickets = await _ticketRepository.ReserveTicketsAsync(eventId, ticketCount);

        // Create booking
        var booking = new Booking
        {
            UserId = userId,
            EventId = eventId,
            TicketCount = ticketCount,
            TotalAmount = eventEntity.Price * ticketCount,
            Status = BookingStatus.Pending,
            BookingDate = DateTime.UtcNow
        };

        var createdBooking = await _bookingRepository.AddAsync(booking);

        // Link tickets to booking
        foreach (var ticket in tickets)
        {
            ticket.BookingId = createdBooking.Id;
            ticket.Status = TicketStatus.Booked;
            await _ticketRepository.UpdateAsync(ticket);
        }

        return createdBooking;
    }

    public async Task<Booking?> GetBookingByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(int userId)
    {
        // Обновляем статусы прошедших билетов перед загрузкой бронирований
        await _ticketRepository.UpdateExpiredTicketsAsync();
        return await _bookingRepository.GetBookingsByUserIdAsync(userId);
    }

    public async Task<bool> ConfirmBookingAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.Status != BookingStatus.Pending)
            return false;

        booking.Status = BookingStatus.Confirmed;
        booking.ConfirmedAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> CancelBookingAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null || booking.Status == BookingStatus.Cancelled)
            return false;

        // Release tickets
        var tickets = await _ticketRepository.GetTicketsByBookingIdAsync(bookingId);
        foreach (var ticket in tickets)
        {
            ticket.Status = TicketStatus.Available;
            ticket.BookingId = null;
            await _ticketRepository.UpdateAsync(ticket);
        }

        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;
        await _bookingRepository.UpdateAsync(booking);
        return true;
    }

    public async Task<bool> BookingExistsAsync(int id)
    {
        return await _bookingRepository.ExistsAsync(id);
    }
}


