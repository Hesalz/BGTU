using System.Windows;
using System.Windows.Controls;

namespace TicketBookingSystem.App.Views;

public partial class EventsView : UserControl
{
    public EventsView()
    {
        InitializeComponent();
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window is MainWindow mainWindow)
        {
            mainWindow.NavigateToProfile();
        }
    }

    private void CategoryToggle_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.ViewModels.EventsViewModel viewModel)
        {
            viewModel.IsCategoryFilterOpen = !viewModel.IsCategoryFilterOpen;
        }
    }

}
