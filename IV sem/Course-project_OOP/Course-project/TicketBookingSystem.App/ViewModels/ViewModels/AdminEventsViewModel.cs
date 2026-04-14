using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class AdminEventsViewModel : ViewModelBase
{
    private readonly IEventService _eventService;
    private ObservableCollection<Event> _events = new();
    private Event? _selectedEvent;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _date = DateTime.Now;
    private TimeSpan _time = DateTime.Now.TimeOfDay;
    private string _location = string.Empty;
    private int _totalTickets;
    private decimal _price;
    private bool _isPriceLocked;
    private string _priceLockMessage = string.Empty;
    public ObservableCollection<EventCategoryOptionViewModel> CategoryOptions { get; }
    public ObservableCollection<EventPhotoEditorItem> PhotoGallery { get; } = new();

    public AdminEventsViewModel(IEventService eventService)
    {
        _eventService = eventService;
        CategoryOptions = new ObservableCollection<EventCategoryOptionViewModel>(
            Enum.GetValues<EventCategory>()
                .Where(c => c != EventCategory.None)
                .Select(c => new EventCategoryOptionViewModel(c, c.GetDisplayName())));
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
                _ = LoadEventDetailsAsync(value);
            }
        }
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public DateTime Date
    {
        get => _date;
        set => SetProperty(ref _date, value);
    }

    public TimeSpan Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public string Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public int TotalTickets
    {
        get => _totalTickets;
        set => SetProperty(ref _totalTickets, value);
    }

    public decimal Price
    {
        get => _price;
        set => SetProperty(ref _price, value);
    }

    public bool IsPriceLocked
    {
        get => _isPriceLocked;
        set => SetProperty(ref _isPriceLocked, value);
    }

    public string PriceLockMessage
    {
        get => _priceLockMessage;
        set => SetProperty(ref _priceLockMessage, value);
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
            
            // Обновляем SelectedEvent на событие из нового списка (без отслеживания),
            // чтобы избежать конфликтов отслеживания
            if (SelectedEvent != null)
            {
                var updatedSelectedEvent = Events.FirstOrDefault(e => e.Id == SelectedEvent.Id);
                if (updatedSelectedEvent != null)
                {
                    SelectedEvent = updatedSelectedEvent;
                }
                else
                {
                    SelectedEvent = null;
                }
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
    private void NewEvent()
    {
        SelectedEvent = null;
        Title = string.Empty;
        Description = string.Empty;
        Date = DateTime.Now;
        Time = DateTime.Now.TimeOfDay;
        Location = string.Empty;
        TotalTickets = 1; // Устанавливаем минимальное валидное значение вместо 0
        Price = 0.01m; // Устанавливаем минимальное валидное значение вместо 0
        PhotoGallery.Clear();
        SyncCategorySelections(EventCategory.None);
        IsPriceLocked = false;
        PriceLockMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveEventAsync()
    {
        try
        {
            ClearError();
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Location) || TotalTickets <= 0 || Price <= 0)
            {
                ErrorMessage = "Заполните все обязательные поля";
                IsLoading = false;
                return;
            }

            Event? savedEvent = null;
            bool isNewEvent = SelectedEvent == null;
            
            if (isNewEvent)
            {
                // Create new event
                var newEvent = new Event
                {
                    Title = Title,
                    Description = Description,
                    Date = Date,
                    Time = Time,
                    Location = Location,
                    TotalTickets = TotalTickets,
                    Price = Price,
                    Categories = GetSelectedCategories(),
                    Photos = CreatePhotoEntities()
                };
                savedEvent = await _eventService.CreateEventAsync(newEvent);
            }
            else
            {
                // Update existing event
                // Создаем новый объект для обновления, чтобы избежать проблем с отслеживанием
                var eventToUpdate = new Event
                {
                    Id = SelectedEvent.Id,
                    Title = Title,
                    Description = Description,
                    Date = Date,
                    Time = Time,
                    Location = Location,
                    TotalTickets = TotalTickets,
                    Price = Price,
                    Categories = GetSelectedCategories(),
                    Photos = CreatePhotoEntities()
                };
                savedEvent = await _eventService.UpdateEventAsync(eventToUpdate);
            }

            // Обновляем список после сохранения
            // Важно: LoadEventsAsync загружает события БЕЗ отслеживания, что предотвращает конфликты
            await LoadEventsAsync();
            
            if (isNewEvent)
            {
                // Для нового события перезагружаем его из базы для получения актуальных данных с фотографиями
                if (savedEvent != null)
                {
                    var newEventFromDb = await _eventService.GetEventByIdAsync(savedEvent.Id);
                    if (newEventFromDb != null)
                    {
                        // Обновляем событие в списке
                        var eventInList = Events.FirstOrDefault(e => e.Id == newEventFromDb.Id);
                        if (eventInList != null)
                        {
                            // Копируем данные из базы
                            eventInList.Title = newEventFromDb.Title;
                            eventInList.Description = newEventFromDb.Description;
                            eventInList.Date = newEventFromDb.Date;
                            eventInList.Time = newEventFromDb.Time;
                            eventInList.Location = newEventFromDb.Location;
                            eventInList.TotalTickets = newEventFromDb.TotalTickets;
                            eventInList.Price = newEventFromDb.Price;
                            eventInList.Categories = newEventFromDb.Categories;
                            eventInList.Photos = newEventFromDb.Photos;
                        }
                    }
                }
                
                // Сбрасываем форму для нового события
                NewEvent();
            }
            else
            {
                // Для существующего события перезагружаем его из базы для получения актуальных данных
                if (savedEvent != null)
                {
                    var updatedEvent = await _eventService.GetEventByIdAsync(savedEvent.Id);
                    if (updatedEvent != null)
                    {
                        // Обновляем событие в списке
                        var eventInList = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
                        if (eventInList != null)
                        {
                            // Копируем обновленные данные
                            eventInList.Title = updatedEvent.Title;
                            eventInList.Description = updatedEvent.Description;
                            eventInList.Date = updatedEvent.Date;
                            eventInList.Time = updatedEvent.Time;
                            eventInList.Location = updatedEvent.Location;
                            eventInList.TotalTickets = updatedEvent.TotalTickets;
                            eventInList.Price = updatedEvent.Price;
                            eventInList.Categories = updatedEvent.Categories;
                            eventInList.Photos = updatedEvent.Photos;
                        }
                        
                        // Обновляем детали в форме
                        // Используем событие из списка (без отслеживания) для предотвращения конфликтов
                        var eventFromList = Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
                        if (eventFromList != null)
                        {
                            SelectedEvent = eventFromList;
                            await LoadEventDetailsAsync(eventFromList);
                        }
                    }
                }
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
    private async Task DeleteEventAsync()
    {
        if (SelectedEvent == null) return;

        try
        {
            ClearError();
            IsLoading = true;
            await _eventService.DeleteEventAsync(SelectedEvent.Id);
            await LoadEventsAsync();
            NewEvent();
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

    private async Task LoadEventDetailsAsync(Event eventEntity)
    {
        // Загружаем событие для редактирования (с отслеживанием)
        // Используем ID из переданного события, чтобы избежать конфликтов отслеживания
        var eventForEdit = await _eventService.GetEventByIdForEditAsync(eventEntity.Id);
        if (eventForEdit == null)
        {
            OnError(new InvalidOperationException("Событие не найдено"));
            return;
        }
        
        // Обновляем SelectedEvent на загруженное событие с отслеживанием
        // Это важно для правильной работы с контекстом
        SelectedEvent = eventForEdit;

        Title = eventForEdit.Title;
        Description = eventForEdit.Description;
        Date = eventForEdit.Date;
        Time = eventForEdit.Time;
        Location = eventForEdit.Location;
        TotalTickets = eventForEdit.TotalTickets;
        Price = eventForEdit.Price;
        PhotoGallery.Clear();
        foreach (var photo in eventForEdit.Photos.OrderBy(p => p.SortOrder))
        {
            PhotoGallery.Add(new EventPhotoEditorItem { ImageData = photo.ImageData });
        }
        SyncCategorySelections(eventForEdit.Categories);

        // Проверяем наличие бронирований
        var hasBookings = await _eventService.HasBookingsAsync(eventEntity.Id);
        IsPriceLocked = hasBookings;
        PriceLockMessage = hasBookings 
            ? "⚠️ Цена заблокирована: у этого мероприятия уже есть бронирования. Изменение цены не будет применено." 
            : string.Empty;
    }

    private EventCategory GetSelectedCategories()
    {
        EventCategory categories = EventCategory.None;
        foreach (var option in CategoryOptions)
        {
            if (option.IsSelected)
            {
                categories |= option.Category;
            }
        }

        return categories;
    }

    private void SyncCategorySelections(EventCategory categories)
    {
        foreach (var option in CategoryOptions)
        {
            option.IsSelected = categories.HasFlag(option.Category);
        }
    }

    [RelayCommand]
    private void AddPhotos()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp",
            Title = "Выберите изображения мероприятия",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            foreach (var fileName in dialog.FileNames)
            {
                PhotoGallery.Add(new EventPhotoEditorItem
                {
                    ImageData = File.ReadAllBytes(fileName)
                });
            }
        }
    }

    [RelayCommand]
    private void RemovePhoto(EventPhotoEditorItem? photo)
    {
        if (photo == null) return;
        PhotoGallery.Remove(photo);
    }

    private List<EventPhoto> CreatePhotoEntities()
    {
        return PhotoGallery
            .Select((photo, index) => new EventPhoto
            {
                ImageData = photo.ImageData,
                SortOrder = index
            })
            .ToList();
    }
}


