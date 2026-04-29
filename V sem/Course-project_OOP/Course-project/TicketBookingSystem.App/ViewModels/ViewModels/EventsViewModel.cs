using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.Services.Repositories;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class EventsViewModel : ViewModelBase
{
    private readonly IEventService _eventService;
    private readonly ITicketRepository _ticketRepository;
    private readonly List<Event> _allLoadedEvents = new();
    private ObservableCollection<Event> _events = new();
    private Event? _selectedEvent;
    private string _searchTerm = string.Empty;
    private Event? _recommendedEvent;
    private readonly Random _random = new();
    private EventSortOption _selectedSortOption = EventSortOption.DateAsc;
    private bool _isGridView;
    private bool _isCategoryFilterOpen;
    private string _categoryFilterSummary = "Все категории";
    public ObservableCollection<EventCategoryOptionViewModel> CategoryFilters { get; }

    public EventsViewModel(IEventService eventService, ITicketRepository ticketRepository)
    {
        _eventService = eventService;
        _ticketRepository = ticketRepository;
        CategoryFilters = new ObservableCollection<EventCategoryOptionViewModel>(
            Enum.GetValues<EventCategory>()
                .Where(c => c != EventCategory.None)
                .Select(c => new EventCategoryOptionViewModel(c, c.GetDisplayName())));

        foreach (var filter in CategoryFilters)
        {
            filter.PropertyChanged += CategoryFilterChanged;
        }

        UpdateCategorySummary();
    }

    public ObservableCollection<Event> Events
    {
        get => _events;
        set => SetProperty(ref _events, value);
    }

    public Event? SelectedEvent
    {
        get => _selectedEvent;
        set => SetProperty(ref _selectedEvent, value);
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            if (SetProperty(ref _searchTerm, value))
            {
                SearchCommand.ExecuteAsync(null);
            }
        }
    }

    public Event? RecommendedEvent
    {
        get => _recommendedEvent;
        set => SetProperty(ref _recommendedEvent, value);
    }

    public EventSortOption SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            if (SetProperty(ref _selectedSortOption, value))
            {
                ApplyFilters();
            }
        }
    }

    public bool IsGridView
    {
        get => _isGridView;
        set => SetProperty(ref _isGridView, value);
    }

    public bool IsCategoryFilterOpen
    {
        get => _isCategoryFilterOpen;
        set => SetProperty(ref _isCategoryFilterOpen, value);
    }

    public string CategoryFilterSummary
    {
        get => _categoryFilterSummary;
        set => SetProperty(ref _categoryFilterSummary, value);
    }

    public event EventHandler<Event>? EventSelected;

    [RelayCommand]
    private async Task LoadEventsAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();
            // Показываем только предстоящие мероприятия для пользователей
            var events = await _eventService.GetUpcomingEventsAsync();
            UpdateSourceEvents(events);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            IEnumerable<Event> events;
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Показываем только предстоящие мероприятия для пользователей
                events = await _eventService.GetUpcomingEventsAsync();
            }
            else
            {
                events = await _eventService.SearchEventsAsync(SearchTerm);
                // Фильтруем прошедшие мероприятия из результатов поиска
                var now = DateTime.Now; // Используем локальное время для сравнения
                events = events.Where(e =>
                {
                    // Объединяем дату и время мероприятия
                    var eventDateOnly = e.Date.Date;
                    var eventDateTime = eventDateOnly.Add(e.Time);
                    // Сравниваем с текущим локальным временем
                    return eventDateTime > now;
                });
            }

            UpdateSourceEvents(events);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SelectEventAsync(Event? eventEntity)
    {
        if (eventEntity == null) return;

        try
        {
            SelectedEvent = eventEntity;
            var availableCount = await _ticketRepository.GetAvailableTicketCountAsync(eventEntity.Id);
            EventSelected?.Invoke(this, eventEntity);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    private void UpdateSourceEvents(IEnumerable<Event> events)
    {
        _allLoadedEvents.Clear();
        _allLoadedEvents.AddRange(events);
        ApplyFilters();
        // Рекомендация обновляется только при явном вызове RefreshRecommendationCommand или при первой загрузке
        // Не обновляем при поиске/фильтрации
    }

    private void ApplyFilters()
    {
        var selectedCategories = GetSelectedCategories();
        var now = DateTime.Now; // Используем локальное время для сравнения
        var filtered = _allLoadedEvents.AsEnumerable()
            // Фильтруем прошедшие мероприятия
            // Объединяем Date и Time для корректного сравнения
            .Where(e =>
            {
                // Объединяем дату и время мероприятия
                var eventDateOnly = e.Date.Date;
                var eventDateTime = eventDateOnly.Add(e.Time);
                // Сравниваем с текущим локальным временем
                // Используем строгое сравнение: мероприятие должно быть в будущем
                var isUpcoming = eventDateTime > now;
                return isUpcoming;
            });

        if (selectedCategories != EventCategory.None)
        {
            filtered = filtered.Where(e => (e.Categories & selectedCategories) != EventCategory.None);
        }
        filtered = SelectedSortOption switch
        {
            EventSortOption.DateAsc => filtered.OrderBy(e => e.Date).ThenBy(e => e.Time),
            EventSortOption.DateDesc => filtered.OrderByDescending(e => e.Date).ThenByDescending(e => e.Time),
            EventSortOption.PriceAsc => filtered.OrderBy(e => e.Price),
            EventSortOption.PriceDesc => filtered.OrderByDescending(e => e.Price),
            _ => filtered
        };

        Events = new ObservableCollection<Event>(filtered);
    }

    private EventCategory GetSelectedCategories()
    {
        EventCategory categoryMask = EventCategory.None;
        foreach (var filter in CategoryFilters)
        {
            if (filter.IsSelected)
            {
                categoryMask |= filter.Category;
            }
        }

        return categoryMask;
    }

    private void CategoryFilterChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EventCategoryOptionViewModel.IsSelected))
        {
            ApplyFilters();
            // Рекомендация не меняется при изменении фильтров
            UpdateCategorySummary();
        }
    }

    [RelayCommand]
    private void RefreshRecommendation()
    {
        if (_allLoadedEvents.Count == 0)
        {
            RecommendedEvent = null;
            return;
        }

        var filtered = Events.ToList();
        if (filtered.Count == 0)
        {
            filtered = _allLoadedEvents;
        }

        RecommendedEvent = filtered[_random.Next(filtered.Count)];
    }

    [RelayCommand]
    private async Task OpenRecommendedAsync()
    {
        if (RecommendedEvent == null)
            return;

        await SelectEventAsync(RecommendedEvent);
    }

    [RelayCommand]
    private void SetSortOption(EventSortOption option)
    {
        SelectedSortOption = option;
    }

    [RelayCommand]
    private void SetViewMode(string mode)
    {
        IsGridView = string.Equals(mode, "grid", StringComparison.OrdinalIgnoreCase);
    }

    [RelayCommand]
    private void ClearCategoryFilters()
    {
        foreach (var filter in CategoryFilters)
        {
            filter.IsSelected = false;
        }

        UpdateCategorySummary();
        ApplyFilters();
        IsCategoryFilterOpen = false;
    }

    private void UpdateCategorySummary()
    {
        var selected = CategoryFilters.Where(f => f.IsSelected).Select(f => f.DisplayName).ToList();
        CategoryFilterSummary = selected.Count == 0 ? "Все категории" : string.Join(", ", selected);
    }
}

public enum EventSortOption
{
    DateAsc,
    DateDesc,
    PriceAsc,
    PriceDesc
}


