using System.Globalization;
using System.Windows.Data;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class TicketStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Available => "Доступен",
                TicketStatus.Booked => "Забронирован",
                TicketStatus.Expired => "Прошедший",
                _ => status.ToString()
            };
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

