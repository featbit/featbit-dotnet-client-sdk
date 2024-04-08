using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Events;
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
            Unloaded += (s, e) =>
            {
                _fbClient.FeatureFlagsUpdated -= FeatureFlagsUpdated;
            };
            //this.DataContext = viewModel;    
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _fbClient.FeatureFlagsUpdated += FeatureFlagsUpdated;
            SetVisibility();
        }

        private void FeatureFlagsUpdated(object? sender, FeatureFlagsUpdatedEventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                SetVisibility();
            }));
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

            double numb = _fbClient.DoubleVariation("float-func", 0);
        }


    }
}