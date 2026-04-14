using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class BookingTicketStatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Booking booking && booking.Tickets != null && booking.Tickets.Any())
        {
            // Проверяем статус билетов
            var hasExpired = booking.Tickets.Any(t => t.Status == TicketStatus.Expired);
            var hasBooked = booking.Tickets.Any(t => t.Status == TicketStatus.Booked);

            if (hasExpired)
                return new SolidColorBrush(Colors.Gray);
            if (hasBooked)
                return new SolidColorBrush(Colors.Orange);
        }

        // Если билетов нет или статус не определен, возвращаем цвет на основе статуса бронирования
        if (value is Booking booking2)
        {
            return booking2.Status switch
            {
                BookingStatus.Pending => new SolidColorBrush(Colors.Orange),
                BookingStatus.Confirmed => new SolidColorBrush(Colors.Green),
                BookingStatus.Completed => new SolidColorBrush(Colors.Blue),
                BookingStatus.Cancelled => new SolidColorBrush(Colors.Red),
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

