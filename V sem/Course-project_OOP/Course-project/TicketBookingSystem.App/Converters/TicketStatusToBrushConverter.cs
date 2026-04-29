using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class TicketStatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TicketStatus status)
        {
            return status switch
            {
                TicketStatus.Available => new SolidColorBrush(Colors.Green),
                TicketStatus.Booked => new SolidColorBrush(Colors.Orange),
                TicketStatus.Expired => new SolidColorBrush(Colors.Gray),
                _ => new SolidColorBrush(Colors.Black)
            };
        }

        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

