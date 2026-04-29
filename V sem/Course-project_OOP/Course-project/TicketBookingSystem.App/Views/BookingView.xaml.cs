using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TicketBookingSystem.ViewModels.ViewModels;

namespace TicketBookingSystem.App.Views;

public partial class BookingView : UserControl
{
    private readonly DispatcherTimer _sliderTimer;

    public BookingView()
    {
        InitializeComponent();
        _sliderTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };
        _sliderTimer.Tick += SliderTimer_Tick;
        DataContextChanged += BookingView_DataContextChanged;
        Unloaded += BookingView_Unloaded;
    }

    private void BookingView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        _sliderTimer.Stop();

        if (e.OldValue is BookingViewModel oldViewModel)
        {
            oldViewModel.EventPhotos.CollectionChanged -= EventPhotos_CollectionChanged;
        }

        if (DataContext is BookingViewModel viewModel)
        {
            viewModel.EventPhotos.CollectionChanged += EventPhotos_CollectionChanged;
            if (viewModel.EventPhotos.Count > 0)
            {
                _sliderTimer.Start();
            }
        }
    }

    private void SliderTimer_Tick(object? sender, EventArgs e)
    {
        if (DataContext is BookingViewModel viewModel)
        {
            viewModel.NextPhotoCommand.Execute(null);
        }
    }

    private void BookingView_Unloaded(object sender, RoutedEventArgs e)
    {
        _sliderTimer.Stop();
        if (DataContext is BookingViewModel viewModel)
        {
            viewModel.EventPhotos.CollectionChanged -= EventPhotos_CollectionChanged;
        }
    }

    private void EventPhotos_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (DataContext is BookingViewModel viewModel && viewModel.EventPhotos.Count > 0)
        {
            _sliderTimer.Start();
        }
        else
        {
            _sliderTimer.Stop();
        }
    }
}

