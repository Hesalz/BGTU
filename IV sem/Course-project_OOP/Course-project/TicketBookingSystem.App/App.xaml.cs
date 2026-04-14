using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TicketBookingSystem.Data;
using TicketBookingSystem.Services.Repositories;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.ViewModels;

namespace TicketBookingSystem.App;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
        EnsureDatabaseCreated();

        // Show login window
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Database
        const string connectionString = "Data Source=PC;Initial Catalog=TicketBookingSystem;TrustServerCertificate=True;Integrated Security=True;";
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Отдельная фабрика контекстов для фоновых операций (например, аналитика),
        // чтобы избежать конфликта "A second operation was started on this context instance..."
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<EventsViewModel>();
        services.AddTransient<BookingViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<AdminDashboardViewModel>();
        services.AddTransient<AdminEventsViewModel>();
        services.AddTransient<AdminTicketsViewModel>();
        services.AddTransient<UserProfileViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
    }

    private void EnsureDatabaseCreated()
    {
        if (_serviceProvider == null) return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Проверяем, существует ли база данных
            if (!context.Database.CanConnect())
            {
                // Создаем базу данных и схему
                context.Database.EnsureCreated();
            }
            else
            {
                // Если база существует, проверяем наличие таблиц с правильной схемой
                try
                {
                    // Пытаемся выполнить простой запрос для проверки схемы
                    var testQuery = context.Users.Take(1).ToList();
                }
                catch (Exception ex)
                {
                    // Если схема не соответствует, предлагаем пересоздать
                    var result = MessageBox.Show(
                        $"Обнаружена проблема со схемой базы данных:\n{ex.Message}\n\n" +
                        "Хотите пересоздать базу данных? (Все данные будут удалены!)",
                        "Проблема со схемой базы данных",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            context.Database.EnsureDeleted();
                            context.Database.EnsureCreated();
                            MessageBox.Show("База данных успешно пересоздана!", "Успех", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception deleteEx)
                        {
                            MessageBox.Show($"Ошибка при пересоздании базы данных: {deleteEx.Message}", 
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            var errorDetails = ex.Message;
            if (ex.InnerException != null)
            {
                errorDetails += $"\nДетали: {ex.InnerException.Message}";
            }
            
            MessageBox.Show(
                $"Ошибка подключения к базе данных:\n{errorDetails}\n\n" +
                "Убедитесь, что:\n" +
                "1. SQL Server запущен\n" +
                "2. База данных 'TicketBookingSystem' существует или может быть создана\n" +
                "3. У вас есть права доступа\n" +
                "4. Строка подключения корректна",
                "Ошибка подключения", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
