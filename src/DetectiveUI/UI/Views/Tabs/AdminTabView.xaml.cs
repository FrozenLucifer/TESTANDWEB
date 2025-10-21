using System.Windows.Controls;
using DetectiveUI.ViewModels;

namespace DetectiveUI.Views.Tabs;

public partial class AdminTabView : UserControl
{
    public AdminTabView(AdminTabViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}