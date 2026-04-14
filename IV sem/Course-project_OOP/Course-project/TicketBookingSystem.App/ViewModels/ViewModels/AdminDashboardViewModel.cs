using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;
using RevenueByMonthDto = TicketBookingSystem.Services.Services.RevenueByMonthDto;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class AdminDashboardViewModel : ViewModelBase
{
    private readonly IAnalyticsService _analyticsService;
    private Dictionary<string, object> _statistics = new();
    private ObservableCollection<KeyValuePair<string, string>> _statisticsDisplay = new();
    private ObservableCollection<PopularEventDto> _popularEvents = new();
    private ObservableCollection<RevenueByMonthDto> _revenueByMonth = new();

    public AdminDashboardViewModel(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    public Dictionary<string, object> Statistics
    {
        get => _statistics;
        set
        {
            SetProperty(ref _statistics, value);
            UpdateStatisticsDisplay();
            UpdateChartData();
        }
    }

    public ObservableCollection<KeyValuePair<string, string>> StatisticsDisplay
    {
        get => _statisticsDisplay;
        set => SetProperty(ref _statisticsDisplay, value);
    }

    public ObservableCollection<PopularEventDto> PopularEvents
    {
        get => _popularEvents;
        set => SetProperty(ref _popularEvents, value);
    }

    public ObservableCollection<RevenueByMonthDto> RevenueByMonth
    {
        get => _revenueByMonth;
        set => SetProperty(ref _revenueByMonth, value);
    }

    private void UpdateStatisticsDisplay()
    {
        var display = new ObservableCollection<KeyValuePair<string, string>>();
        
        foreach (var kvp in Statistics)
        {
            // Пропускаем служебные ключи
            if (kvp.Key == "PopularEvents" || kvp.Key == "RevenueByMonth")
                continue;
            
            string valueStr = kvp.Value switch
            {
                int i => i.ToString(),
                decimal d => d.ToString("C", CultureInfo.GetCultureInfo("en-US")),
                double db => db.ToString("F2"),
                string s => s,
                _ => kvp.Value?.ToString() ?? "N/A"
            };
            
            display.Add(new KeyValuePair<string, string>(kvp.Key, valueStr));
        }
        
        StatisticsDisplay = display;
    }

    private void UpdateChartData()
    {
        // Обработка PopularEvents
        if (Statistics.TryGetValue("PopularEvents", out var popularEventsObj))
        {
            try
            {
                var popularEvents = popularEventsObj switch
                {
                    List<PopularEventDto> list => list,
                    IEnumerable<PopularEventDto> enumerable => enumerable.ToList(),
                    _ => new List<PopularEventDto>()
                };
                PopularEvents = new ObservableCollection<PopularEventDto>(popularEvents);
            }
            catch (Exception ex)
            {
                PopularEvents = new ObservableCollection<PopularEventDto>();
            }
        }
        else
        {
            PopularEvents = new ObservableCollection<PopularEventDto>();
        }

        // Обработка RevenueByMonth
        if (Statistics.TryGetValue("RevenueByMonth", out var revenueObj))
        {
            try
            {
                var revenueList = revenueObj switch
                {
                    List<RevenueByMonthDto> list => list,
                    IEnumerable<RevenueByMonthDto> enumerable => enumerable.ToList(),
                    _ => new List<RevenueByMonthDto>()
                };
                
                RevenueByMonth = new ObservableCollection<RevenueByMonthDto>(revenueList);
                MaxRevenue = revenueList.Any() ? revenueList.Max(r => r.Revenue) : 1000;
            }
            catch (Exception ex)
            {
                RevenueByMonth = new ObservableCollection<RevenueByMonthDto>();
                MaxRevenue = 1000;
            }
        }
        else
        {
            RevenueByMonth = new ObservableCollection<RevenueByMonthDto>();
            MaxRevenue = 1000;
        }
    }

    private decimal _maxRevenue = 1000;
    public decimal MaxRevenue
    {
        get => _maxRevenue;
        set => SetProperty(ref _maxRevenue, value);
    }

    public event EventHandler? NavigateToEvents;
    public event EventHandler? NavigateToTickets;
    public event EventHandler? RecreateDatabaseRequested;

    [RelayCommand]
    private async Task LoadStatisticsAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();
            Statistics = await _analyticsService.GetDashboardStatisticsAsync();
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

    public void NavigateToEventsView()
    {
        NavigateToEvents?.Invoke(this, EventArgs.Empty);
    }

    public void NavigateToTicketsView()
    {
        NavigateToTickets?.Invoke(this, EventArgs.Empty);
    }

    public void RequestRecreateDatabase()
    {
        RecreateDatabaseRequested?.Invoke(this, EventArgs.Empty);
    }
}


