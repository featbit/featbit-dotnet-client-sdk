using FeatBit.ClientSdk;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IFbClient _fbClient;
        public MainWindow(
            IFbClient fbClient, 
            MainViewModel viewModel)
        {
            InitializeComponent();
            _fbClient = fbClient;
            Loaded += MainWindow_Loaded;
            //this.DataContext = viewModel;    
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetVisibility();
        }

        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            SetVisibility();
        }

        private void SetVisibility()
        {
            var visibleStatus = _fbClient.StringVariation("testing-visibility", "Collapsed");
            switch (visibleStatus)
            {
                case "Visible":
                    TextBox_Thanks.Visibility = Visibility.Visible;
                    break;
                case "Collapsed":
                    TextBox_Thanks.Visibility = Visibility.Collapsed;
                    break;
                default:
                    TextBox_Thanks.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}