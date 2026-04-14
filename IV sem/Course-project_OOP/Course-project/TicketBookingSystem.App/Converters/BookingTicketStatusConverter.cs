using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class BookingTicketStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Booking booking && booking.Tickets != null && booking.Tickets.Any())
        {
            // Проверяем статус билетов
            var hasExpired = booking.Tickets.Any(t => t.Status == TicketStatus.Expired);
            var hasBooked = booking.Tickets.Any(t => t.Status == TicketStatus.Booked);
            var hasAvailable = booking.Tickets.Any(t => t.Status == TicketStatus.Available);

            if (hasExpired)
                return "Прошедший";
            if (hasBooked)
                return "Забронирован";
            if (hasAvailable)
                return "Доступен";
        }

        // Если билетов нет или статус не определен, возвращаем статус бронирования
        if (value is Booking booking2)
        {
            return booking2.Status switch
            {
                BookingStatus.Pending => "Забронирован",
                BookingStatus.Confirmed => "Забронирован",
                BookingStatus.Completed => "Завершено",
                BookingStatus.Cancelled => "Отменено",
                _ => "Забронирован"
            };
        }

        return "Неизвестно";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

