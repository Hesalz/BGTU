using System.Collections.ObjectModel;
using System.Windows;
using TicketBookingSystem.App.Views;
using CommunityToolkit.Mvvm.Input;
using TicketBookingSystem.Models;
using TicketBookingSystem.Models.Enums;
using TicketBookingSystem.Services.Repositories;
using TicketBookingSystem.Services.Services;
using TicketBookingSystem.ViewModels.Base;

namespace TicketBookingSystem.ViewModels.ViewModels;

public partial class UserProfileViewModel : ViewModelBase
{
    private readonly IUserRepository _userRepository;
    private readonly IBookingService _bookingService;
    private readonly IAuthenticationService _authenticationService;
    private User? _currentUser;
    private string _email = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string? _phoneNumber;
    private string _oldPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmNewPassword = string.Empty;
    private ObservableCollection<Booking> _bookings = new();
    private Booking? _selectedBooking;
    private string? _profileSuccessMessage;
    private string? _profileErrorMessage;
    private string? _passwordSuccessMessage;
    private string? _passwordErrorMessage;

    public UserProfileViewModel(
        IUserRepository userRepository,
        IBookingService bookingService,
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _bookingService = bookingService;
        _authenticationService = authenticationService;
    }

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            if (SetProperty(ref _currentUser, value) && value != null)
            {
                LoadUserData(value);
            }
        }
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
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

    public string OldPassword
    {
        get => _oldPassword;
        set => SetProperty(ref _oldPassword, value);
    }

    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    public string ConfirmNewPassword
    {
        get => _confirmNewPassword;
        set => SetProperty(ref _confirmNewPassword, value);
    }

    public ObservableCollection<Booking> Bookings
    {
        get => _bookings;
        set
        {
            if (SetProperty(ref _bookings, value))
            {
                OnPropertyChanged(nameof(HasBookings));
                // Подписываемся на изменения коллекции
                if (_bookings != null)
                {
                    _bookings.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasBookings));
                }
            }
        }
    }

    public bool HasBookings => Bookings != null && Bookings.Count > 0;

    public Booking? SelectedBooking
    {
        get => _selectedBooking;
        set => SetProperty(ref _selectedBooking, value);
    }

    public string? ProfileSuccessMessage
    {
        get => _profileSuccessMessage;
        set => SetProperty(ref _profileSuccessMessage, value);
    }

    public string? ProfileErrorMessage
    {
        get => _profileErrorMessage;
        set => SetProperty(ref _profileErrorMessage, value);
    }

    public string? PasswordSuccessMessage
    {
        get => _passwordSuccessMessage;
        set => SetProperty(ref _passwordSuccessMessage, value);
    }

    public string? PasswordErrorMessage
    {
        get => _passwordErrorMessage;
        set => SetProperty(ref _passwordErrorMessage, value);
    }

    public event EventHandler? NavigateBack;
    public event EventHandler? PasswordChangedSuccessfully;

    [RelayCommand]
    private async Task LoadBookingsAsync()
    {
        if (CurrentUser == null) return;

        try
        {
            IsLoading = true;
            ClearError();
            var bookings = await _bookingService.GetUserBookingsAsync(CurrentUser.Id);
            var newBookings = new ObservableCollection<Booking>(bookings);
            newBookings.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasBookings));
            Bookings = newBookings;
            OnPropertyChanged(nameof(HasBookings));
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
    private async Task SaveProfileAsync()
    {
        if (CurrentUser == null) return;

        try
        {
            ProfileErrorMessage = null;
            ProfileSuccessMessage = null;
            IsLoading = true;

            var email = Email?.Trim() ?? string.Empty;
            var firstName = FirstName?.Trim() ?? string.Empty;
            var lastName = LastName?.Trim() ?? string.Empty;
            var phoneNumber = PhoneNumber?.Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                ProfileErrorMessage = "Email, имя и фамилия обязательны";
                return;
            }

            // Проверяем, не занят ли email другим пользователем
            if (email != CurrentUser.Email)
            {
                if (await _userRepository.EmailExistsAsync(email))
                {
                    ProfileErrorMessage = "Этот email уже используется";
                    return;
                }
            }

            CurrentUser.Email = email;
            CurrentUser.FirstName = firstName;
            CurrentUser.LastName = lastName;
            CurrentUser.PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber;

            await _userRepository.UpdateAsync(CurrentUser);
            ProfileSuccessMessage = "✅ Изменения успешно сохранены!";
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                ProfileSuccessMessage = null;
            });
        }
        catch (Exception ex)
        {
            OnError(ex);
            ProfileErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (CurrentUser == null) return;

        try
        {
            ClearError();
            IsLoading = true;

            var oldPassword = OldPassword?.Trim() ?? string.Empty;
            var newPassword = NewPassword?.Trim() ?? string.Empty;
            var confirmPassword = ConfirmNewPassword?.Trim() ?? string.Empty;
            PasswordErrorMessage = null;
            PasswordSuccessMessage = null;

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                PasswordErrorMessage = "Заполните все поля";
                return;
            }

            if (newPassword != confirmPassword)
            {
                PasswordErrorMessage = "Новые пароли не совпадают";
                return;
            }

            if (newPassword.Length < 6)
            {
                PasswordErrorMessage = "Пароль должен содержать минимум 6 символов";
                return;
            }

            var success = await _authenticationService.ChangePasswordAsync(CurrentUser.Id, oldPassword, newPassword);
            if (!success)
            {
                PasswordErrorMessage = "Неверный текущий пароль";
                return;
            }

            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            PasswordSuccessMessage = "✅ Пароль успешно изменен!";
            PasswordChangedSuccessfully?.Invoke(this, EventArgs.Empty);
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                PasswordSuccessMessage = null;
                PasswordErrorMessage = null;
            });
        }
        catch (Exception ex)
        {
            OnError(ex);
            PasswordErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CancelBookingAsync()
    {
        if (SelectedBooking == null) return;

        try
        {
            ClearError();
            IsLoading = true;

            var success = await _bookingService.CancelBookingAsync(SelectedBooking.Id);
            if (!success)
            {
                ErrorMessage = "Не удалось отменить бронирование";
                return;
            }

            await LoadBookingsAsync();
            SelectedBooking = null;
            ShowSuccess("✅ Бронирование успешно отменено!");
            // Автоматически скрываем сообщение через 3 секунды
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                ClearSuccess();
            });
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
    private void Back()
    {
        NavigateBack?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void OpenChangePasswordWindow()
    {
        // Очищаем поля перед открытием окна
        OldPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
        PasswordErrorMessage = null;
        PasswordSuccessMessage = null;
        
        // Открываем окно смены пароля
        var window = new ChangePasswordWindow(this);
        window.Owner = Application.Current.MainWindow;
        window.ShowDialog();
    }

    public Task InitializeAsync(User user)
    {
        CurrentUser = user;
        return LoadBookingsAsync();
    }

    private void LoadUserData(User user)
    {
        Email = user.Email;
        FirstName = user.FirstName;
        LastName = user.LastName;
        PhoneNumber = user.PhoneNumber;
    }
}


