using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private string _email = string.Empty;
    private string _password = string.Empty;

    public LoginViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public event EventHandler<User>? LoginSuccessful;
    public event EventHandler? NavigateToRegister;

    [RelayCommand]
    private async Task LoginAsync()
    {
        try
        {
            ClearError();
            IsLoading = true;

            var email = Email?.Trim() ?? string.Empty;
            var password = Password?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Email и пароль обязательны";
                return;
            }

            var user = await _authenticationService.LoginAsync(email, password);
            if (user == null)
            {
                ErrorMessage = "Неверный email или пароль";
                return;
            }

            LoginSuccessful?.Invoke(this, user);
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

    [RelayCommand]
    private void NavigateToRegisterView()
    {
        NavigateToRegister?.Invoke(this, EventArgs.Empty);
    }
}


