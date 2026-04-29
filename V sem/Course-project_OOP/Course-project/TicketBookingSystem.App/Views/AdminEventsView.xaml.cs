using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using TicketBookingSystem.ViewModels.ViewModels;

namespace TicketBookingSystem.App.Views;

public partial class AdminEventsView : UserControl
{
    public AdminEventsView()
    {
        InitializeComponent();
        Loaded += AdminEventsView_Loaded;
    }

    private void AdminEventsView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is AdminEventsViewModel viewModel)
        {
            TimeTextBox.Text = viewModel.Time.ToString(@"hh\:mm");
        }
    }

    private void TimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is AdminEventsViewModel viewModel && sender is TextBox textBox)
        {
            if (TimeSpan.TryParseExact(textBox.Text, @"hh\:mm", CultureInfo.InvariantCulture, out var timeSpan))
            {
                viewModel.Time = timeSpan;
            }
        }
    }
}
