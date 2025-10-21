using System.Windows.Controls;
using DetectiveUI.ViewModels;

namespace DetectiveUI.Views.Tabs;

public partial class ProfileTabView : UserControl
{
    public ProfileTabView(ProfileTabViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}