using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authenticationService;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string? _phoneNumber;

    public RegisterViewModel(IAuthenticationService authenticationService)
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

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    public event EventHandler<User>? RegistrationSuccessful;
    public event EventHandler? NavigateToLogin;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        try
        {
            ClearError();
            IsLoading = true;

            var email = Email?.Trim() ?? string.Empty;
            var password = Password?.Trim() ?? string.Empty;
            var confirmPassword = ConfirmPassword?.Trim() ?? string.Empty;
            var firstName = FirstName?.Trim() ?? string.Empty;
            var lastName = LastName?.Trim() ?? string.Empty;
            var phoneNumber = PhoneNumber?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                ErrorMessage = "Все обязательные поля должны быть заполнены";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage = "Пароль должен содержать минимум 6 символов";
                return;
            }

            var user = await _authenticationService.RegisterAsync(email, password, firstName, lastName,
                string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber);
            RegistrationSuccessful?.Invoke(this, user);
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var innerException = ex.InnerException?.Message ?? ex.Message;
            ErrorMessage = $"Ошибка сохранения данных: {innerException}";
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            if (ex.InnerException != null)
            {
                errorMsg += $"\nДетали: {ex.InnerException.Message}";
            }
            ErrorMessage = $"Ошибка: {errorMsg}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NavigateToLoginView()
    {
        NavigateToLogin?.Invoke(this, EventArgs.Empty);
    }
}

