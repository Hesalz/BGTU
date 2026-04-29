using System.Globalization;
using System.Windows.Data;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class BookingStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BookingStatus status)
            {
                return status switch
                {
                    BookingStatus.Pending => "Забронирован",
                    BookingStatus.Confirmed => "Забронирован",
                    BookingStatus.Completed => "Завершено",
                    BookingStatus.Cancelled => "Отменено",
                    _ => "Забронирован"
                };
            }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

