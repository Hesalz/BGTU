using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TicketBookingSystem.ViewModels.ViewModels;

namespace TicketBookingSystem.App.Views;

public partial class ChangePasswordWindow : Window
{
    private readonly UserProfileViewModel _viewModel;

    public ChangePasswordWindow(UserProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        
        // Подписываемся на событие успешной смены пароля
        _viewModel.PasswordChangedSuccessfully += OnPasswordChangedSuccessfully;
    }

    private void OldPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.OldPassword = passwordBox.Password;
        }
    }

    private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.NewPassword = passwordBox.Password;
        }
    }

    private void ConfirmNewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.ConfirmNewPassword = passwordBox.Password;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        // Очищаем поля при закрытии
        _viewModel.OldPassword = string.Empty;
        _viewModel.NewPassword = string.Empty;
        _viewModel.ConfirmNewPassword = string.Empty;
        _viewModel.PasswordErrorMessage = null;
        _viewModel.PasswordSuccessMessage = null;
        
        Close();
    }

    private void OnPasswordChangedSuccessfully(object? sender, EventArgs e)
    {
        // Закрываем окно через 2 секунды после успешной смены пароля
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            Dispatcher.Invoke(() =>
            {
                Close();
            });
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        // Отписываемся от события
        _viewModel.PasswordChangedSuccessfully -= OnPasswordChangedSuccessfully;
        base.OnClosed(e);
    }
}

