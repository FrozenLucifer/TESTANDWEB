using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using DetectiveUI.ApiClient;
using DetectiveUI.ViewModels;
using DetectiveUI.Views;
using DetectiveUI.Views.Tabs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Serializers.Json;

namespace DetectiveUI;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }
    public IConfiguration Configuration { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();
        services.AddSingleton<IConfiguration>(Configuration);

        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow();
        mainWindow.Show();

        var navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Initialize(mainWindow);
        navigationService.NavigateTo<LoginView>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.IncludeFields = false;
        options.Converters.Add(new JsonStringEnumConverter());

        string baseUrl = Configuration["ApiConfiguration:BaseUrl"];
        services.AddSingleton(new RestClient(baseUrl,
            configureSerialization: s => s.UseSystemTextJson(options)));

        services.AddSingleton<IApiClient, HttpApiClient>();
        services.AddSingleton<TokenService>();
        services.AddSingleton<INavigationService, NavigationService>();

        services.AddTransient<ReadTabView>();
        services.AddTransient<EditTabView>();
        services.AddTransient<AdminTabView>();
        services.AddTransient<ProfileTabView>();

        services.AddTransient<MainAppViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<AdminTabViewModel>();
        services.AddTransient<ProfileTabViewModel>();
        services.AddTransient<ReadTabViewModel>();
        services.AddTransient<EditTabViewModel>();

        services.AddTransient<LoginView>();
        services.AddTransient<MainAppViewModel>();
        services.AddTransient<MainAppView>();
    }
}