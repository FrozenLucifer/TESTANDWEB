using System.Windows.Controls;
using DetectiveUI.ViewModels;

namespace DetectiveUI.Views.Tabs;

public partial class EditTabView : UserControl
{
    public EditTabView(EditTabViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}