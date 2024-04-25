using FeatBit.ClientSdk;
using System.Net;
using System.Windows;
using FeatBit.ClientSdk.Model;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IFbClient _fbClient;
        private readonly MainWindow _mainWindow;

        public LoginWindow(
            MainWindow mainWindow,
            IFbClient fbClient)
        {
            InitializeComponent();
            _fbClient = fbClient;
            _mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // You can initialize feature flags from a local file
            _fbClient.InitFeatureFlagsFromLocal(new List<FeatureFlag>() { });

            // You can save the feature flags to a local file by
            // 1. Calling _fbClient.GetLatestAll();
            // 2. Save to your local file

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBox_UserName.Text))
            {
                MessageBox.Show("Please enter a valid user name.");
                return;
            }
            var userName = TextBox_UserName.Text;

            var user = FbUser.Builder(userName.Trim().Replace(" ", ""))
                                .Name(userName)
                                .Build();
            await _fbClient.IdentifyAsync(user);

            _mainWindow.Show();
    
            this.Close();
        }
    }
}
