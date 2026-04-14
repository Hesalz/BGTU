using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TicketBookingSystem.App.Views;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.ViewModels.ViewModels;

namespace TicketBookingSystem.App;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;
    private MainViewModel? _mainViewModel;

    public CommunityToolkit.Mvvm.Input.IAsyncRelayCommand LogoutCommand { get; }

    public MainWindow(IServiceProvider serviceProvider)
    {
        LogoutCommand = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(LogoutAsync);
        InitializeComponent();
        _serviceProvider = serviceProvider;
        InitializeNavigation();
    }

    private void InitializeNavigation()
    {
        _mainViewModel = _serviceProvider.GetRequiredService<MainViewModel>();
        DataContext = _mainViewModel;

        // Start with login view
        var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
        loginViewModel.LoginSuccessful += OnLoginSuccessful;
        loginViewModel.NavigateToRegister += OnNavigateToRegister;
        _mainViewModel.NavigateTo(loginViewModel);
        ContentControl.Content = new LoginView { DataContext = loginViewModel };
    }

    private void OnLoginSuccessful(object? sender, User user)
    {
        if (_mainViewModel == null) return;

        _mainViewModel.CurrentUser = user;

        if (user.Role == UserRole.Admin)
        {
            NavigateToAdminDashboard();
        }
        else
        {
            NavigateToEvents();
        }
    }

    private void OnNavigateToRegister(object? sender, EventArgs e)
    {
        if (_mainViewModel == null) return;

        var registerViewModel = _serviceProvider.GetRequiredService<RegisterViewModel>();
        registerViewModel.RegistrationSuccessful += OnRegistrationSuccessful;
        registerViewModel.NavigateToLogin += OnNavigateToLogin;
        _mainViewModel.NavigateTo(registerViewModel);
        ContentControl.Content = new RegisterView { DataContext = registerViewModel };
    }

    private void OnRegistrationSuccessful(object? sender, User user)
    {
        if (_mainViewModel == null) return;

        _mainViewModel.CurrentUser = user;
        NavigateToEvents();
    }

    private void OnNavigateToLogin(object? sender, EventArgs e)
    {
        if (_mainViewModel == null) return;

        var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
        loginViewModel.LoginSuccessful += OnLoginSuccessful;
        loginViewModel.NavigateToRegister += OnNavigateToRegister;
        _mainViewModel.NavigateTo(loginViewModel);
        ContentControl.Content = new LoginView { DataContext = loginViewModel };
    }

    private void NavigateToEvents()
    {
        if (_mainViewModel == null) return;

        var eventsViewModel = _serviceProvider.GetRequiredService<EventsViewModel>();
        eventsViewModel.EventSelected += OnEventSelected;
        _mainViewModel.NavigateTo(eventsViewModel);
        ContentControl.Content = new EventsView { DataContext = eventsViewModel };
        eventsViewModel.LoadEventsCommand.ExecuteAsync(null);
    }

    public void NavigateToProfile()
    {
        if (_mainViewModel?.CurrentUser == null) return;

        var profileViewModel = _serviceProvider.GetRequiredService<UserProfileViewModel>();
        profileViewModel.NavigateBack += OnNavigateBackToEvents;
        _ = profileViewModel.InitializeAsync(_mainViewModel.CurrentUser);
        _mainViewModel.NavigateTo(profileViewModel);
        ContentControl.Content = new UserProfileView { DataContext = profileViewModel };
    }

    private void OnNavigateBackToEvents(object? sender, EventArgs e)
    {
        NavigateToEvents();
    }

    private void NavigateToAdminDashboard()
    {
        if (_mainViewModel == null) return;

        var dashboardViewModel = _serviceProvider.GetRequiredService<AdminDashboardViewModel>();
        dashboardViewModel.NavigateToEvents += OnNavigateToAdminEvents;
        dashboardViewModel.NavigateToTickets += OnNavigateToAdminTickets;
        dashboardViewModel.RecreateDatabaseRequested += OnRecreateDatabaseRequested;
        _mainViewModel.NavigateTo(dashboardViewModel);
        
        var view = new AdminDashboardView { DataContext = dashboardViewModel };
        ContentControl.Content = view;
        
        // Убеждаемся, что DataContext установлен
        System.Diagnostics.Debug.WriteLine($"NavigateToAdminDashboard: ViewModel type = {dashboardViewModel.GetType().Name}");
        System.Diagnostics.Debug.WriteLine($"NavigateToAdminDashboard: View DataContext = {view.DataContext?.GetType().Name ?? "null"}");
        
        dashboardViewModel.LoadStatisticsCommand.ExecuteAsync(null);
    }

    private void OnNavigateToAdminEvents(object? sender, EventArgs e)
    {
        if (_mainViewModel == null) return;

        var eventsViewModel = _serviceProvider.GetRequiredService<AdminEventsViewModel>();
        eventsViewModel.NavigateBack += OnNavigateBackToAdminDashboard;
        _mainViewModel.NavigateTo(eventsViewModel);
        ContentControl.Content = new AdminEventsView { DataContext = eventsViewModel };
        eventsViewModel.LoadEventsCommand.ExecuteAsync(null);
    }

    private void OnNavigateToAdminTickets(object? sender, EventArgs e)
    {
        if (_mainViewModel == null) return;

        var ticketsViewModel = _serviceProvider.GetRequiredService<AdminTicketsViewModel>();
        ticketsViewModel.NavigateBack += OnNavigateBackToAdminDashboard;
        _mainViewModel.NavigateTo(ticketsViewModel);
        ContentControl.Content = new AdminTicketsView { DataContext = ticketsViewModel };
        ticketsViewModel.LoadEventsCommand.ExecuteAsync(null);
    }

    private void OnNavigateBackToAdminDashboard(object? sender, EventArgs e)
    {
        NavigateToAdminDashboard();
    }

    private Task LogoutAsync()
    {
        if (_mainViewModel == null) return Task.CompletedTask;

        var result = MessageBox.Show(
            "Вы уверены, что хотите выйти из аккаунта?",
            "Выход из аккаунта",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _mainViewModel.Logout();
            InitializeNavigation();
        }
        
        return Task.CompletedTask;
    }

    private async void OnRecreateDatabaseRequested(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "ВНИМАНИЕ! Все данные в базе данных будут удалены!\n\n" +
            "Это действие нельзя отменить. Вы уверены, что хотите пересоздать базу данных?",
            "Подтверждение пересоздания базы данных",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TicketBookingSystem.Data.ApplicationDbContext>();

            // Удаляем и пересоздаем базу данных
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            MessageBox.Show(
                "База данных успешно пересоздана!\n\n" +
                "Все таблицы созданы заново. Администратор по умолчанию:\n" +
                "Email: admin@ticketbooking.com\n" +
                "Пароль: Admin123!",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Обновляем статистику
            if (sender is AdminDashboardViewModel dashboardViewModel)
            {
                _ = dashboardViewModel.LoadStatisticsCommand.ExecuteAsync(null);
            }
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            if (ex.InnerException != null)
            {
                errorMsg += $"\nДетали: {ex.InnerException.Message}";
            }

            MessageBox.Show(
                $"Ошибка при пересоздании базы данных:\n{errorMsg}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void OnEventSelected(object? sender, Models.Event eventEntity)
    {
        if (_mainViewModel?.CurrentUser == null) return;

        var bookingViewModel = _serviceProvider.GetRequiredService<BookingViewModel>();
        bookingViewModel.BookingCreated += OnBookingCreated;
        bookingViewModel.BookingCancelled += OnBookingCancelled;
        bookingViewModel.InitializeAsync(eventEntity, _mainViewModel.CurrentUser.Id);
        _mainViewModel.NavigateTo(bookingViewModel);
        ContentControl.Content = new BookingView { DataContext = bookingViewModel };
    }

    private void OnBookingCreated(object? sender, Models.Booking booking)
    {
        MessageBox.Show($"Бронирование создано! Номер бронирования: {booking.Id}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        NavigateToEvents();
    }

    private void OnBookingCancelled(object? sender, EventArgs e)
    {
        NavigateToEvents();
    }
}
