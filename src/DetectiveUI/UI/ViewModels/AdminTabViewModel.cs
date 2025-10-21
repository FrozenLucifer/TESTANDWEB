using System.Windows.Input;
using DetectiveUI.ApiClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using DetectiveUI.Views;
using DTOs;
using DTOs.Enum;

namespace DetectiveUI.ViewModels;

public class AdminTabViewModel : INotifyPropertyChanged
{
    private readonly IApiClient _apiClient;

    public ObservableCollection<UserDto> Users { get; } = new();

    public List<UserTypeDto> UserTypes { get; } = Enum.GetValues<UserTypeDto>().ToList();

    public event PropertyChangedEventHandler PropertyChanged;

    public string NewUsername { get; set; }
    public UserTypeDto SelectedUserType { get; set; }
    public string SelectedUsername { get; set; }

    public ICommand GetUsersCommand { get; }
    public ICommand CreateUserCommand { get; }
    public ICommand DeleteUserCommand { get; }
    public ICommand ResetPasswordCommand { get; }

    private string _tempPassword;

    public string TempPassword
    {
        get => _tempPassword;
        set
        {
            _tempPassword = value;
            OnPropertyChanged();
        }
    }

    private bool _showPassword;

    public bool ShowPassword
    {
        get => _showPassword;
        set
        {
            _showPassword = value;
            OnPropertyChanged();
        }
    }

    public ICommand CopyPasswordCommand { get; }

    public AdminTabViewModel(IApiClient apiClient)
    {
        _apiClient = apiClient;

        GetUsersCommand = new RelayCommand(async () => await GetUsers());
        CreateUserCommand = new RelayCommand(async () => await CreateUser());
        DeleteUserCommand = new RelayCommand(async () => await DeleteUser());
        ResetPasswordCommand = new RelayCommand(async () => await ResetPassword());
        CopyPasswordCommand = new RelayCommand(CopyPassword);
    }

    private async Task GetUsers()
    {
        var users = await _apiClient.GetUsersAsync();
        Users.Clear();

        foreach (var user in users)
        {
            Users.Add(user);
        }
    }

    private async Task CreateUser()
    {
        if (string.IsNullOrWhiteSpace(NewUsername)) return;

        try
        {
            TempPassword = await _apiClient.CreateUserAsync(NewUsername, SelectedUserType);
            ShowPassword = true;
            await GetUsers();
        }
        catch (Exception ex)
        {
            TempPassword = $"Ошибка: {ex.Message}";
            ShowPassword = true;
        }
    }

    private async Task ResetPassword()
    {
        if (string.IsNullOrWhiteSpace(SelectedUsername)) return;

        try
        {
            TempPassword = await _apiClient.ResetPasswordAsync(SelectedUsername);
            ShowPassword = true;
        }
        catch (Exception ex)
        {
            TempPassword = $"Ошибка: {ex.Message}";
            ShowPassword = true;
        }
    }

    private void CopyPassword()
    {
        if (!string.IsNullOrEmpty(TempPassword))
        {
            Clipboard.SetText(TempPassword);
        }
    }


    private async Task DeleteUser()
    {
        if (string.IsNullOrWhiteSpace(SelectedUsername)) return;

        await _apiClient.DeleteUserAsync(SelectedUsername);
        await GetUsers();
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}