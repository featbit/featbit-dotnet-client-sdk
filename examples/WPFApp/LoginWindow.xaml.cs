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
            var fakeUser = FbUser.Builder("a-unique-key-of-fake-user-003")
                            .Name("Fake User 003")
                            .Custom("age", "15")
                            .Custom("country", "FR")
                            .Build();

            try
            {
                var task = Task.Run(async () =>
                {
                    await _fbClient.IdentifyAsync(fakeUser, autoSync: true);
                }).Wait(TimeSpan.FromSeconds(10));
                if (task == false)
                {
                    MessageBox.Show("Failed to identify user. Please try again.");
                    return;
                }
                _mainModel.Show();
                this.Close();
            }
            catch(Exception exp)
            {

            }
        }
    }
}
