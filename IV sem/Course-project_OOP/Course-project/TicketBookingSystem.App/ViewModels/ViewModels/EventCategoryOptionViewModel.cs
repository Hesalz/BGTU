using CommunityToolkit.Mvvm.ComponentModel;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class EventCategoryOptionViewModel : ObservableObject
{
    public EventCategoryOptionViewModel(EventCategory category, string displayName)
    {
        Category = category;
        DisplayName = displayName;
    }

    public EventCategory Category { get; }

    public string DisplayName { get; }

    [ObservableProperty]
    private bool _isSelected;
}


