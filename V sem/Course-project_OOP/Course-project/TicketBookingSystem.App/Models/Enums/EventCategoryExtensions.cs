using System;
using System.Collections.Generic;
using System.Linq;

namespace TicketBookingSystem.Models.Enums;

public static class EventCategoryExtensions
{
    private static readonly IReadOnlyDictionary<EventCategory, string> DisplayNames = new Dictionary<EventCategory, string>
    {
        { EventCategory.Music, "Музыка" },
        { EventCategory.Art, "Искусство" },
        { EventCategory.ActiveLeisure, "Активный отдых" },
        { EventCategory.Education, "Обучение" },
        { EventCategory.Food, "Еда" },
        { EventCategory.Technology, "Технологии" }
    };

    public static string GetDisplayName(this EventCategory category)
    {
        if (DisplayNames.TryGetValue(category, out var name))
        {
            return name;
        }

        return category.ToString();
    }

    public static IEnumerable<EventCategory> GetIndividualCategories(this EventCategory categories)
    {
        foreach (var category in Enum.GetValues<EventCategory>())
        {
            if (category == EventCategory.None)
                continue;

            if (categories.HasFlag(category))
                yield return category;
        }
    }

    public static string ToDisplayString(this EventCategory categories)
    {
        var names = categories.GetIndividualCategories()
            .Select(GetDisplayName)
            .ToList();

        return names.Count == 0 ? "Без категорий" : string.Join(", ", names);
    }
}


