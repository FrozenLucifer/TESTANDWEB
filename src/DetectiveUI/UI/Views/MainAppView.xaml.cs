using System.Windows.Controls;
using DetectiveUI.ViewModels;

namespace DetectiveUI.Views;

public partial class MainAppView : Page
{
    public MainAppView(MainAppViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}