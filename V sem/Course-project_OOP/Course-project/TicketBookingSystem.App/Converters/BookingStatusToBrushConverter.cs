using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class BookingStatusToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush GreenBrush = new(Color.FromRgb(34, 139, 34));
    private static readonly SolidColorBrush RedBrush = new(Color.FromRgb(220, 53, 69));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is BookingStatus status)
        {
            return status == BookingStatus.Cancelled ? RedBrush : GreenBrush;
        }

        return GreenBrush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}


