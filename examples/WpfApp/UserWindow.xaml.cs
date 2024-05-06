using System.Windows;

namespace WpfApp;

public partial class UserWindow : Window
{
    public UserWindow()
    {
        InitializeComponent();
        DataContext = new UserViewMode();
    }
}