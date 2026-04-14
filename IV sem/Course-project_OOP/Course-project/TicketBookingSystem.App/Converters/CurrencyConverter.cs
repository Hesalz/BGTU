using System;
using System.Globalization;
using System.Windows.Data;

namespace TicketBookingSystem.App.Converters;

public class CurrencyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        if (value is decimal decimalValue)
        {
            return decimalValue.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        if (value is double doubleValue)
        {
            return doubleValue.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        if (value is int intValue)
        {
            return intValue.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        if (decimal.TryParse(value.ToString(), out var parsedValue))
        {
            return parsedValue.ToString("C", CultureInfo.GetCultureInfo("en-US"));
        }

        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

