using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TicketBookingSystem.App.Converters;

public class RevenueToHeightConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
            return 20.0;

        decimal revenue = 0;
        decimal maxRevenue = 1000;

        // Первое значение - Revenue
        if (values[0] is decimal rev)
        {
            revenue = rev;
        }
        else if (values[0] != null && decimal.TryParse(values[0].ToString(), out var parsedRev))
        {
            revenue = parsedRev;
        }

        // Второе значение - MaxRevenue
        if (values[1] is decimal max)
        {
            maxRevenue = max;
        }
        else if (values[1] != null && decimal.TryParse(values[1].ToString(), out var parsedMax))
        {
            maxRevenue = parsedMax;
        }

        if (maxRevenue <= 0) maxRevenue = 1000;

        // Максимальная высота столбца - 180 пикселей
        const double maxHeight = 180.0;
        
        // Масштабируем пропорционально
        var height = (double)revenue / (double)maxRevenue * maxHeight;
        return Math.Max(Math.Min(height, maxHeight), 20.0); // Минимальная высота 20, максимальная 180
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

