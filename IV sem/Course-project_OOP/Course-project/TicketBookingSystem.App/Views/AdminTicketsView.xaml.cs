using System.Windows;
using System.Windows.Controls;
using TicketBookingSystem.Models.Enums;

namespace TicketBookingSystem.App.Views;

public partial class AdminTicketsView : UserControl
{
    public AdminTicketsView()
    {
        InitializeComponent();
    }

    private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item && DataContext is ViewModels.ViewModels.AdminTicketsViewModel viewModel)
        {
            var tag = item.Tag?.ToString();
            viewModel.FilterByStatus(tag);
        }
    }
}

