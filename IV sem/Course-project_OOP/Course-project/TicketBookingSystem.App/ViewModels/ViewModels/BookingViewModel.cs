using System.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Services.Repositories;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class BookingViewModel : ViewModelBase
{
    private readonly IBookingService _bookingService;
    private readonly ITicketRepository _ticketRepository;
    private Event? _selectedEvent;
    private int _ticketCount = 1;
    private decimal _totalAmount;
    private int _currentPhotoIndex;
    private byte[]? _currentPhoto;
    public ObservableCollection<byte[]> EventPhotos { get; } = new();

    public BookingViewModel(IBookingService bookingService, ITicketRepository ticketRepository)
    {
        _bookingService = bookingService;
        _ticketRepository = ticketRepository;
    }

    public Event? SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            if (SetProperty(ref _selectedEvent, value))
            {
                UpdateTotalAmount();
            }
        }
    }

    public int TicketCount
    {
        get => _ticketCount;
        set
        {
            if (SetProperty(ref _ticketCount, value))
            {
                UpdateTotalAmount();
                // Очищаем ошибку, если значение стало валидным
                if (value > 0 && !string.IsNullOrEmpty(ErrorMessage) && 
                    (ErrorMessage.Contains("количество билетов") || ErrorMessage.Contains("один билет")))
                {
                    ClearError();
                }
            }
        }
    }

    public decimal TotalAmount
    {
        get => _totalAmount;
        set => SetProperty(ref _totalAmount, value);
    }

    public byte[]? CurrentPhoto
    {
        get => _currentPhoto;
        set => SetProperty(ref _currentPhoto, value);
    }

    public int CurrentPhotoIndex
    {
        get => _currentPhotoIndex;
        private set => SetProperty(ref _currentPhotoIndex, value);
    }

    public int? AvailableTickets { get; private set; }

    public event EventHandler<Booking>? BookingCreated;
    public event EventHandler? BookingCancelled;

    public async Task InitializeAsync(Event eventEntity, int userId)
    {
        SelectedEvent = eventEntity;
        UserId = userId;
        TicketCount = 1;

        try
        {
            AvailableTickets = await _ticketRepository.GetAvailableTicketCountAsync(eventEntity.Id);
            OnPropertyChanged(nameof(AvailableTickets));
            UpdateTotalAmount();
            LoadPhotos(eventEntity);
        }
        catch (Exception ex)
        {
            OnError(ex);
        }
    }

    private int UserId { get; set; }

    private void UpdateTotalAmount()
    {
        if (SelectedEvent != null)
        {
            TotalAmount = SelectedEvent.Price * TicketCount;
        }
    }

    [RelayCommand]
    private async Task ConfirmBookingAsync()
    {
        if (SelectedEvent == null)
        {
            ErrorMessage = "Мероприятие не выбрано";
            return;
        }

        // Валидация количества билетов перед отправкой
        if (TicketCount <= 0)
        {
            if (TicketCount == 0)
            {
                ErrorMessage = "Необходимо выбрать хотя бы один билет для бронирования.";
            }
            else
            {
                ErrorMessage = $"Количество билетов не может быть отрицательным числом. Пожалуйста, введите положительное число.";
            }
            return;
        }

        try
        {
            ClearError();
            IsLoading = true;

            var booking = await _bookingService.CreateBookingAsync(UserId, SelectedEvent.Id, TicketCount);
            BookingCreated?.Invoke(this, booking);
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
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
    private void CancelBooking()
    {
        BookingCancelled?.Invoke(this, EventArgs.Empty);
    }

    private void LoadPhotos(Event eventEntity)
    {
        EventPhotos.Clear();
        var photos = eventEntity.Photos?
            .OrderBy(p => p.SortOrder)
            .Select(p => p.ImageData)
            .Where(p => p is { Length: > 0 }) ?? Enumerable.Empty<byte[]>();

        foreach (var photo in photos)
        {
            EventPhotos.Add(photo);
        }

        CurrentPhotoIndex = 0;
        CurrentPhoto = EventPhotos.FirstOrDefault();
    }

    [RelayCommand]
    private void NextPhoto()
    {
        if (EventPhotos.Count == 0) return;
        CurrentPhotoIndex = (CurrentPhotoIndex + 1) % EventPhotos.Count;
        CurrentPhoto = EventPhotos[CurrentPhotoIndex];
    }

    [RelayCommand]
    private void PreviousPhoto()
    {
        if (EventPhotos.Count == 0) return;
        CurrentPhotoIndex = (CurrentPhotoIndex - 1 + EventPhotos.Count) % EventPhotos.Count;
        CurrentPhoto = EventPhotos[CurrentPhotoIndex];
    }
}


