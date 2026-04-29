using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.Services.Services;

public interface IAnalyticsService
{
    Task<int> GetTotalBookingsCountAsync();
    Task<decimal> GetTotalRevenueAsync();
    Task<IEnumerable<PopularEventDto>> GetPopularEventsAsync(int topCount = 10);
    Task<Dictionary<string, object>> GetDashboardStatisticsAsync();
}

public class PopularEventDto
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public decimal Revenue { get; set; }
    public EventCategory Categories { get; set; }
}

public class RevenueByMonthDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}


