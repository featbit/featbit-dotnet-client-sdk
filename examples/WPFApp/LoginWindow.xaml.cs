using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Models;
using System.Windows;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IFbClient _fbClient;
        private readonly MainWindow _mainModel;
        public LoginWindow(
            MainWindow mainWindow,
            IFbClient fbClient)
        {
            InitializeComponent();
            _mainModel = mainWindow;
            _fbClient = fbClient;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fakeUser = FbUser.Builder("a-unique-key-of-fake-user-001")
                            .Name("Fake User 001")
                            .Custom("age", "15")
                            .Custom("country", "FR")
                            .Build();
            _fbClient.Identify(fakeUser);
            _mainModel.Show();
            this.Close();
        }
    }
}
