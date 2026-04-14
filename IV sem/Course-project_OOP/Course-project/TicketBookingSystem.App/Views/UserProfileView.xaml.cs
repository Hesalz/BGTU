using System.Windows;
using System.Windows.Controls;
using TicketBookingSystem.ViewModels.ViewModels;
using TicketBookingSystem.App;

namespace TicketBookingSystem.App.Views;

public partial class UserProfileView : UserControl
{
    public UserProfileView()
    {
        InitializeComponent();
    }


    private void CancelBookingButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is TicketBookingSystem.Models.Booking booking)
        {
            if (DataContext is UserProfileViewModel viewModel)
            {
                viewModel.SelectedBooking = booking;
                _ = viewModel.CancelBookingCommand.ExecuteAsync(null);
            }
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is MainWindow mainWindow)
        {
            mainWindow.LogoutCommand.ExecuteAsync(null);
        }
    }
}

