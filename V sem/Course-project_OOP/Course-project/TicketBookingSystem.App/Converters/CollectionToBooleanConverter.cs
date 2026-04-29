using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace TicketBookingSystem.App.Converters;

public class CollectionToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            // Если значение null, для Inverse возвращаем true (показать "нет бронирований")
            return parameter?.ToString() == "Inverse";
        }

        // Если это строка, не обрабатываем как коллекцию
        if (value is string str)
        {
            var hasValue = !string.IsNullOrEmpty(str);
            if (parameter?.ToString() == "Inverse")
                return !hasValue;
            return hasValue;
        }

        // Если это коллекция, проверяем количество элементов
        if (value is IEnumerable enumerable)
        {
            bool hasItems = false;
            
            try
            {
                // Сначала пробуем получить Count через рефлексию (работает для ObservableCollection, List и т.д.)
                var countProperty = value.GetType().GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);
                if (countProperty != null && countProperty.CanRead && countProperty.PropertyType == typeof(int))
                {
                    var count = (int)countProperty.GetValue(value)!;
                    hasItems = count > 0;
                }
                // Если нет свойства Count, пробуем ICollection
                else if (value is ICollection collection)
                {
                    hasItems = collection.Count > 0;
                }
                // Если и это не сработало, перебираем коллекцию
                else
                {
                    var enumerator = enumerable.GetEnumerator();
                    hasItems = enumerator.MoveNext();
                    
                    if (enumerator is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            catch
            {
                // Если не удалось проверить, считаем что коллекция пуста
                hasItems = false;
            }
            
            if (parameter?.ToString() == "Inverse")
                return !hasItems;
            return hasItems;
        }

        // Для других типов считаем что значение есть
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

