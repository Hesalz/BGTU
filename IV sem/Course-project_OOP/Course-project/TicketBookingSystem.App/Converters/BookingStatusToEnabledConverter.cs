using System.Globalization;
using System.Windows.Data;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class BookingStatusToEnabledConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BookingStatus status)
        {
            // Можно отменить только ожидающие или подтвержденные бронирования
            return status == BookingStatus.Pending || status == BookingStatus.Confirmed;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

