using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace TicketBookingSystem.App.Converters;

public class NullToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return parameter?.ToString() == "Inverse";

        // Если это коллекция, проверяем, есть ли элементы
        if (value is IEnumerable enumerable && !(value is string))
        {
            var hasItems = enumerable.Cast<object>().Any();
            if (parameter?.ToString() == "Inverse")
                return !hasItems;
            return hasItems;
        }

        var result = value != null;
        if (parameter?.ToString() == "Inverse")
            return !result;
        return result;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

