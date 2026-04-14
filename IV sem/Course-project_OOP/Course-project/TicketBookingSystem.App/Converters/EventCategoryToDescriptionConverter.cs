using System;
using System.Globalization;
using System.Windows.Data;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Converters;

public class EventCategoryToDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is EventCategory category)
        {
            return category.ToDisplayString();
        }

        return "Без категорий";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}


