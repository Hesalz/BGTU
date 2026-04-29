using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TicketBookingSystem.App.Views;

public partial class AdminDashboardView : UserControl
{
    public AdminDashboardView()
    {
        InitializeComponent();
        Loaded += AdminDashboardView_Loaded;
    }

    private void AdminDashboardView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext == null)
        {
            System.Diagnostics.Debug.WriteLine("AdminDashboardView: DataContext is null!");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"AdminDashboardView: DataContext type = {DataContext.GetType().Name}");
        }
    }

    private void NavigateToEventsButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TicketBookingSystem.ViewModels.ViewModels.AdminDashboardViewModel viewModel)
        {
            viewModel.NavigateToEventsView();
        }
    }

    private void NavigateToTicketsButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TicketBookingSystem.ViewModels.ViewModels.AdminDashboardViewModel viewModel)
        {
            viewModel.NavigateToTicketsView();
        }
    }

    private void RecreateDatabaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is TicketBookingSystem.ViewModels.ViewModels.AdminDashboardViewModel viewModel)
        {
            viewModel.RequestRecreateDatabase();
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window is MainWindow mainWindow)
        {
            mainWindow.LogoutCommand.ExecuteAsync(null);
        }
    }
}
