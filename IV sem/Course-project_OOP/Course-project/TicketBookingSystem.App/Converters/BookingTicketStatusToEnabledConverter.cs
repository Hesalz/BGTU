using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class BookingTicketStatusToEnabledConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Booking booking)
        {
            // Нельзя отменить, если бронирование уже отменено
            if (booking.Status == BookingStatus.Cancelled)
                return false;

            // Проверяем статус билетов
            if (booking.Tickets != null && booking.Tickets.Any())
            {
                // Если есть прошедшие билеты, нельзя отменить
                var hasExpired = booking.Tickets.Any(t => t.Status == TicketStatus.Expired);
                if (hasExpired)
                    return false;
            }

            // Можно отменить только ожидающие или подтвержденные бронирования
            return booking.Status == BookingStatus.Pending || booking.Status == BookingStatus.Confirmed;
        }

        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

