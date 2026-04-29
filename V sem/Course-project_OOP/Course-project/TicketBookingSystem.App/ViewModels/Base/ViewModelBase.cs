using CommunityToolkit.Mvvm.ComponentModel;

namespace TicketBookingSystem.ViewModels.Base;

public abstract class ViewModelBase : ObservableObject
{
    private bool _isLoading;
    private string? _errorMessage;
    private string? _successMessage;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    protected virtual void OnError(Exception ex)
    {
        ErrorMessage = ex.Message;
        SuccessMessage = null;
    }

    protected virtual void ClearError()
    {
        ErrorMessage = null;
    }

    protected virtual void ClearSuccess()
    {
        SuccessMessage = null;
    }

    protected virtual void ShowSuccess(string message)
    {
        SuccessMessage = message;
        ErrorMessage = null;
    }
}


