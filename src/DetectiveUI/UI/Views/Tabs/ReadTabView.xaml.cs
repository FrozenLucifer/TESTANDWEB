using System.Windows.Controls;
using DetectiveUI.ViewModels;

namespace DetectiveUI.Views.Tabs;

public partial class ReadTabView : UserControl
{
    public ReadTabView(ReadTabViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}