using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TicketBookingSystem.App.Converters;
using TicketBookingSystem.Models;

namespace TicketBookingSystem.App.Views;

public partial class EventPhotoSlider : UserControl
{
    private readonly DispatcherTimer _sliderTimer;
    private EventPhotoSliderViewModel? _viewModel;

    public EventPhotoSlider()
    {
        InitializeComponent();
        _sliderTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };
        _sliderTimer.Tick += SliderTimer_Tick;
        DataContextChanged += EventPhotoSlider_DataContextChanged;
        Unloaded += EventPhotoSlider_Unloaded;
    }

    private void EventPhotoSlider_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _sliderTimer.Stop();

        if (e.OldValue is EventPhotoSliderViewModel oldViewModel)
        {
            oldViewModel.Photos.CollectionChanged -= Photos_CollectionChanged;
        }

        if (DataContext is EventPhotoSliderViewModel viewModel)
        {
            _viewModel = viewModel;
            viewModel.Photos.CollectionChanged += Photos_CollectionChanged;
            if (viewModel.Photos.Count > 1)
            {
                _sliderTimer.Start();
            }
        }
        else if (DataContext is Models.Event eventEntity)
        {
            // Если DataContext - это Event, создаем ViewModel
            _viewModel = new EventPhotoSliderViewModel { Event = eventEntity };
            DataContext = _viewModel;
            _viewModel.Photos.CollectionChanged += Photos_CollectionChanged;
            // Запускаем таймер, если есть несколько фото
            if (_viewModel.Photos.Count > 1)
            {
                _sliderTimer.Start();
            }
            else
            {
                _sliderTimer.Stop();
            }
        }
        else
        {
            _viewModel = null;
            _sliderTimer.Stop();
        }
    }

    private void SliderTimer_Tick(object? sender, EventArgs e)
    {
        _viewModel?.NextPhotoCommand.Execute(null);
    }

    private void EventPhotoSlider_Unloaded(object sender, RoutedEventArgs e)
    {
        _sliderTimer.Stop();
        if (_viewModel != null)
        {
            _viewModel.Photos.CollectionChanged -= Photos_CollectionChanged;
        }
    }

    private void Photos_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_viewModel != null && _viewModel.Photos.Count > 1)
        {
            _sliderTimer.Start();
        }
        else
        {
            _sliderTimer.Stop();
        }
    }
}

public class EventPhotoSliderViewModel : System.ComponentModel.INotifyPropertyChanged
{
    private byte[]? _currentPhoto;
    private int _currentPhotoIndex;
    private Event? _event;

    public EventPhotoSliderViewModel()
    {
        Photos = new ObservableCollection<byte[]>();
    }

    public Event? Event
    {
        get => _event;
        set
        {
            _event = value;
            if (value != null)
            {
                LoadPhotos(value);
            }
            OnPropertyChanged(nameof(Event));
        }
    }

    public ObservableCollection<byte[]> Photos { get; }
    public bool HasMultiplePhotos => Photos.Count > 1;

    public byte[]? CurrentPhoto
    {
        get => _currentPhoto;
        set
        {
            _currentPhoto = value;
            OnPropertyChanged(nameof(CurrentPhoto));
        }
    }

    public int CurrentPhotoIndex
    {
        get => _currentPhotoIndex;
        set
        {
            _currentPhotoIndex = value;
            CurrentPhoto = Photos.Count > 0 ? Photos[_currentPhotoIndex] : null;
            OnPropertyChanged(nameof(CurrentPhotoIndex));
        }
    }

    private void LoadPhotos(Event eventEntity)
    {
        Photos.Clear();
        
        if (eventEntity?.Photos == null || !eventEntity.Photos.Any())
        {
            CurrentPhoto = null;
            CurrentPhotoIndex = 0;
            OnPropertyChanged(nameof(HasMultiplePhotos));
            OnPropertyChanged(nameof(CurrentPhoto));
            return;
        }

        var photos = eventEntity.Photos
            .OrderBy(p => p.SortOrder)
            .Select(p => p.ImageData)
            .Where(p => p is { Length: > 0 })
            .ToList();

        foreach (var photo in photos)
        {
            Photos.Add(photo);
        }

        CurrentPhotoIndex = 0;
        CurrentPhoto = Photos.FirstOrDefault();
        OnPropertyChanged(nameof(HasMultiplePhotos));
        OnPropertyChanged(nameof(CurrentPhoto));
        
        // Убеждаемся, что таймер запущен, если есть несколько фото
        if (Photos.Count > 1)
        {
            // Таймер будет запущен из EventPhotoSlider_DataContextChanged
        }
    }

    public CommunityToolkit.Mvvm.Input.RelayCommand NextPhotoCommand => new(() =>
    {
        if (Photos.Count == 0) return;
        CurrentPhotoIndex = (CurrentPhotoIndex + 1) % Photos.Count;
    });

    public CommunityToolkit.Mvvm.Input.RelayCommand PreviousPhotoCommand => new(() =>
    {
        if (Photos.Count == 0) return;
        CurrentPhotoIndex = (CurrentPhotoIndex - 1 + Photos.Count) % Photos.Count;
    });

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }
}

