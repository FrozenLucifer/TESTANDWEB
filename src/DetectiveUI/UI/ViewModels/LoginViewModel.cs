using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using DetectiveUI.ApiClient;
using DetectiveUI.Views;

namespace DetectiveUI.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IApiClient _apiClient;
    private readonly TokenService _tokenService;
    private readonly INavigationService _navigationService;

    private string _username = "Admin";
    private string _password = "Admin";
    private string _errorMessage;
    private bool _isLoading = false;

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand LoginCommand { get; }

    public LoginViewModel(IApiClient apiClient, TokenService tokenService, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _tokenService = tokenService;
        _navigationService = navigationService;

        LoginCommand = new RelayCommand(async () => await Login());
    }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
            ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
            ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
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

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            ((RelayCommand)LoginCommand).NotifyCanExecuteChanged();
        }
    }

    private bool CanLogin() => !IsLoading &&
                               !string.IsNullOrWhiteSpace(Username) &&
                               !string.IsNullOrWhiteSpace(Password);

    private async Task Login()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var response = await _apiClient.LoginAsync(Username, Password);
            _tokenService.SetToken(response.Token);

            _navigationService.NavigateTo<MainAppView>();
        }
        catch (ApiException ex) when (ex.StatusCode == (HttpStatusCode)403)
        {
            ErrorMessage = "Invalid username or password";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}