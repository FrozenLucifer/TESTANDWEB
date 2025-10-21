using System.Windows.Controls;
using DetectiveUI.ViewModels;

namespace DetectiveUI.Views;

public partial class LoginView : Page
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}