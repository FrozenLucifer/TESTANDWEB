using DetectiveUI.ApiClient;
using DetectiveUI.Views.Tabs;
using Microsoft.Extensions.DependencyInjection;

namespace DetectiveUI.ViewModels;

public class MainAppViewModel
{
    private readonly TokenService _tokenService;
    private readonly IServiceProvider _serviceProvider;

    public List<TabItem> AvailableTabs { get; }

    public MainAppViewModel(TokenService tokenService, IServiceProvider serviceProvider)
    {
        _tokenService = tokenService;
        _serviceProvider = serviceProvider;
        AvailableTabs = GetAvailableTabs();
    }

    private List<TabItem> GetAvailableTabs()
    {
        var allTabs = new List<(string Header, Func<object> Content, string[] AllowedRoles)>
        {
            ("Чтение", () => _serviceProvider.GetRequiredService<ReadTabView>(), new[] { "Admin", "Employee", "SpecialUser" }),
            ("Изменение", () => _serviceProvider.GetRequiredService<EditTabView>(), new[] { "Admin", "Employee" }),
            ("Пользователи", () => _serviceProvider.GetRequiredService<AdminTabView>(), new[] { "Admin" }),
            ("Профиль", () => _serviceProvider.GetRequiredService<ProfileTabView>(), new[] { "Admin", "Employee", "SpecialUser" })
        };

        var currentRole = _tokenService.GetRoleClaim();

        var availableTabs = allTabs
            .Where(tab => tab.AllowedRoles.Contains(currentRole))
            .Select(tab => new TabItem 
            { 
                Header = tab.Header, 
                Content = tab.Content() 
            })
            .ToList();

        return availableTabs;
    }}

public class TabItem
{
    public string Header { get; set; }
    public object Content { get; set; }
}