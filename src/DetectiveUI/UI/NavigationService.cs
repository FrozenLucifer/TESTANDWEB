using System.Windows;
using System.Windows.Controls;
using DetectiveUI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DetectiveUI;

public interface INavigationService
{
    void Initialize(Window mainWindow);
    void NavigateTo<T>() where T : Page;
}

public class NavigationService : INavigationService
{
    private Frame _mainFrame;
    private IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Initialize(Window mainWindow)
    {
        if (mainWindow is not MainWindow mw) 
            throw new ArgumentException("MainWindow expected");
            
        _mainFrame = mw.MainFrame;
    }
    
    public void NavigateTo<T>() where T : Page
    {
        if (_mainFrame == null)
            throw new InvalidOperationException("MainFrame is not initialized");
        
        var page = _serviceProvider.GetRequiredService<T>();
        _mainFrame.Navigate(page);
    }
}