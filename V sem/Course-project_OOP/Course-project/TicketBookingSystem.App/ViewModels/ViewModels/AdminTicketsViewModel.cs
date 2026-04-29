using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.Services.Repositories;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class AdminTicketsViewModel : ViewModelBase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IEventService _eventService;
    private ObservableCollection<Ticket> _tickets = new();
    private ObservableCollection<Ticket> _filteredTickets = new();
    private ObservableCollection<Event> _events = new();
    private Event? _selectedEvent;
    private Ticket? _selectedTicket;
    private string? _statusFilter;
    private int _totalTicketsCount;
    private int _availableTicketsCount;
    private int _bookedTicketsCount;

    public AdminTicketsViewModel(ITicketRepository ticketRepository, IEventService eventService)
    {
        _ticketRepository = ticketRepository;
        _eventService = eventService;
    }

    public ObservableCollection<Ticket> Tickets
    {
        get => _tickets;
        set
        {
            SetProperty(ref _tickets, value);
            ApplyFilter();
            UpdateStatistics();
        }
    }

    public ObservableCollection<Ticket> FilteredTickets
    {
        get => _filteredTickets;
        set => SetProperty(ref _filteredTickets, value);
    }

    public int TotalTicketsCount
    {
        get => _totalTicketsCount;
        set => SetProperty(ref _totalTicketsCount, value);
    }

    public int AvailableTicketsCount
    {
        get => _availableTicketsCount;
        set => SetProperty(ref _availableTicketsCount, value);
    }

    public int BookedTicketsCount
    {
        get => _bookedTicketsCount;
        set => SetProperty(ref _bookedTicketsCount, value);
    }

    public ObservableCollection<Event> Events
    {
        get => _events;
        set => SetProperty(ref _events, value);
    }

    public Event? SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            if (SetProperty(ref _selectedEvent, value) && value != null)
            {
                LoadTicketsForEvent(value.Id);
            }
            else
            {
                Tickets = new ObservableCollection<Ticket>();
                FilteredTickets = new ObservableCollection<Ticket>();
                UpdateStatistics();
            }
        }
    }

    public Ticket? SelectedTicket
    {
        get => _selectedTicket;
        set => SetProperty(ref _selectedTicket, value);
    }

    public event EventHandler? NavigateBack;

    [RelayCommand]
    private async Task LoadEventsAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();
            var events = await _eventService.GetAllEventsAsync();
            Events = new ObservableCollection<Event>(events);
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
    private async Task LoadTicketsForEventAsync(int eventId)
    {
        try
        {
            IsLoading = true;
            ClearError();
            // Обновляем статусы прошедших билетов перед загрузкой
            await _ticketRepository.UpdateExpiredTicketsAsync();
            var tickets = await _ticketRepository.GetTicketsByEventIdAsync(eventId);
            Tickets = new ObservableCollection<Ticket>(tickets);
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

    public void FilterByStatus(string? status)
    {
        _statusFilter = status;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrEmpty(_statusFilter))
        {
            FilteredTickets = new ObservableCollection<Ticket>(Tickets);
        }
        else
        {
            var status = Enum.Parse<TicketStatus>(_statusFilter);
            FilteredTickets = new ObservableCollection<Ticket>(Tickets.Where(t => t.Status == status));
        }
    }

    private void UpdateStatistics()
    {
        if (SelectedEvent == null)
        {
            TotalTicketsCount = 0;
            AvailableTicketsCount = 0;
            BookedTicketsCount = 0;
            return;
        }

        TotalTicketsCount = Tickets.Count;
        AvailableTicketsCount = Tickets.Count(t => t.Status == TicketStatus.Available);
        BookedTicketsCount = Tickets.Count(t => t.Status == TicketStatus.Booked);
    }

    private async void LoadTicketsForEvent(int eventId)
    {
        await LoadTicketsForEventAsync(eventId);
    }

    [RelayCommand]
    private async Task DeleteTicketAsync()
    {
        if (SelectedTicket == null) return;

        try
        {
            ClearError();
            IsLoading = true;
            await _ticketRepository.DeleteAsync(SelectedTicket.Id);
            if (SelectedEvent != null)
            {
                await LoadTicketsForEventAsync(SelectedEvent.Id);
            }
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
    private void Back()
    {
        NavigateBack?.Invoke(this, EventArgs.Empty);
    }
}


