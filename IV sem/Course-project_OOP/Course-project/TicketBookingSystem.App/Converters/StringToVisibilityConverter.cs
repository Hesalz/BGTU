using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TicketBookingSystem.App.Converters;

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            var isEmpty = string.IsNullOrWhiteSpace(str);
            if (parameter?.ToString() == "Inverse")
                return isEmpty ? Visibility.Visible : Visibility.Collapsed;
            return isEmpty ? Visibility.Collapsed : Visibility.Visible;
        }
        if (parameter?.ToString() == "Inverse")
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

