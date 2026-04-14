using CommunityToolkit.Mvvm.ComponentModel;
using TicketBookingSystem.Models;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private User? _currentUser;

    public MainViewModel()
    {
    }

    public void NavigateTo(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
    }

    public event EventHandler? LogoutRequested;

    public void Logout()
    {
        CurrentUser = null;
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}


