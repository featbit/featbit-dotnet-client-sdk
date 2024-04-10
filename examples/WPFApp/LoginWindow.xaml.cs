using FeatBit.ClientSdk;
using System.Net;
using System.Windows;

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
