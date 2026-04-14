using System.Windows;
using System.Windows.Controls;
using TicketBookingSystem.ViewModels.ViewModels;

namespace TicketBookingSystem.App.Views;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
        Loaded += RegisterView_Loaded;
    }

    private void RegisterView_Loaded(object sender, RoutedEventArgs e)
    {
        PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        ConfirmPasswordBox.PasswordChanged += ConfirmPasswordBox_PasswordChanged;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel && PasswordBox != null)
        {
            viewModel.Password = PasswordBox.Password;
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel && ConfirmPasswordBox != null)
        {
            viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
        }
    }
}

