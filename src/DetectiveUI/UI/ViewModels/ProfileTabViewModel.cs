using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DetectiveUI.ApiClient;
using DetectiveUI.Views;

namespace DetectiveUI.ViewModels;

public class ProfileTabViewModel : INotifyPropertyChanged
{
    private readonly IApiClient _apiClient;
    private readonly TokenService _tokenService;
    private readonly INavigationService _navigationService;

    private string _username;
    private string _oldPassword;
    private string _newPassword;
    private string _confirmPassword;
    private string _errorMessage;
    private string _successMessage;

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand ChangePasswordCommand { get; }
    public ICommand LogoutCommand { get; }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public string OldPassword
    {
        get => _oldPassword;
        set
        {
            _oldPassword = value;
            OnPropertyChanged();
            ((RelayCommand)ChangePasswordCommand).NotifyCanExecuteChanged();
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set
        {
            _newPassword = value;
            OnPropertyChanged();
            ((RelayCommand)ChangePasswordCommand).NotifyCanExecuteChanged();
        }
    }

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            _confirmPassword = value;
            OnPropertyChanged();
            ((RelayCommand)ChangePasswordCommand).NotifyCanExecuteChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string SuccessMessage
    {
        get => _successMessage;
        set
        {
            _successMessage = value;
            OnPropertyChanged();
        }
    }

    public ProfileTabViewModel(IApiClient apiClient, TokenService tokenService, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _tokenService = tokenService;
        _navigationService = navigationService;
        
        var token = tokenService.GetParsedToken();
        Username = token?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        ChangePasswordCommand = new RelayCommand(async () => await ChangePassword(), CanChangePassword);
        LogoutCommand = new RelayCommand(Logout);
    }

    private bool CanChangePassword()
    {
        return !string.IsNullOrWhiteSpace(OldPassword) &&
               !string.IsNullOrWhiteSpace(NewPassword) &&
               !string.IsNullOrWhiteSpace(ConfirmPassword) &&
               NewPassword == ConfirmPassword;
    }

    private async Task ChangePassword()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            await _apiClient.ChangePasswordAsync(Username, OldPassword, NewPassword);
            
            SuccessMessage = "Password changed successfully!";
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error changing password: {ex.Message}";
        }
    }

    private void Logout()
    {
        _tokenService.ClearToken();
        _navigationService.NavigateTo<LoginView>();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}