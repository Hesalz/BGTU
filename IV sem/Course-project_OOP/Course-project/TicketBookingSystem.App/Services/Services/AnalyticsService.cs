using Microsoft.EntityFrameworkCore;
using TicketBookingSystem.Data;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public AnalyticsService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> GetTotalBookingsCountAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Bookings.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
            .SumAsync(b => b.TotalAmount);
    }

    public async Task<IEnumerable<PopularEventDto>> GetPopularEventsAsync(int topCount = 10)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Bookings
            .Include(b => b.Event)
            .Where(b => b.Status != BookingStatus.Cancelled) // Учитываем все бронирования кроме отмененных
            .GroupBy(b => new { b.EventId, b.Event.Title, b.Event.Categories })
            .Select(g => new PopularEventDto
            {
                EventId = g.Key.EventId,
                EventTitle = g.Key.Title,
                Categories = g.Key.Categories,
                BookingCount = g.Count(),
                Revenue = g.Sum(b => b.TotalAmount)
            })
            .OrderByDescending(e => e.BookingCount)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetDashboardStatisticsAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        // Материализуем все запросы на этом локальном контексте, чтобы избежать гонок
        var bookings = await context.Bookings
            .Include(b => b.Event)
            .ToListAsync();
        var events = await context.Events.ToListAsync();
        
        var now = DateTime.UtcNow;
        
        // Общая выручка считается для всех бронирований (кроме отмененных)
        // Учитываются как прошедшие, так и будущие мероприятия
        var totalRevenue = bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .Sum(b => b.TotalAmount);
        
        var totalBookings = bookings.Count;
        var totalEvents = events.Count;
        // Подсчитываем предстоящие мероприятия с учетом даты и времени
        var nowLocal = DateTime.Now; // Используем локальное время для сравнения
        var upcomingEvents = events
            .Count(e =>
            {
                // Объединяем дату и время мероприятия
                var eventDateOnly = e.Date.Date;
                var eventDateTime = eventDateOnly.Add(e.Time);
                // Сравниваем с текущим локальным временем
                return eventDateTime > nowLocal;
            });

        var popularEvents = await GetPopularEventsAsync(5);
        
        // Выручка по месяцам для всех бронирований (кроме отмененных)
        var revenueByMonth = bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .GroupBy(b => new { b.BookingDate.Year, b.BookingDate.Month })
            .Select(g => new RevenueByMonthDto
            {
                Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                Revenue = g.Sum(b => b.TotalAmount)
            })
            .OrderBy(x => x.Month)
            .Take(6)
            .ToList();

        return new Dictionary<string, object>
        {
            { "Всего бронирований", totalBookings },
            { "Общая выручка", totalRevenue },
            { "Всего мероприятий", totalEvents },
            { "Предстоящих мероприятий", upcomingEvents },
            { "PopularEvents", popularEvents },
            { "RevenueByMonth", revenueByMonth }
        };
    }
}


