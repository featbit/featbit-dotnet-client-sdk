using System;
using System.Windows;
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.ChangeTracker;
using FeatBit.Sdk.Client.Model;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FbClient _client = FeatBit.Instance;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var evalDetail = _client.StringVariationDetail("welcome-visibilty", "Visible");
            GameRunner.Text = $"Welcome visibilty: {evalDetail.Value}, reason: {evalDetail.Reason}";

            var flagTracker = _client.FlagTracker;

            Subscriber subscriber = valueChangedEvent =>
            {
                Dispatcher.Invoke(() =>
                {
                    GameRunner.Text =
                        $"Welcome visibilty: {valueChangedEvent.NewValue} (Old value: {valueChangedEvent.OldValue})";
                });
            };

            flagTracker.Subscribe("welcome-visibilty", subscriber);
        }

        private async void Identify_Click(object sender, RoutedEventArgs e)
        {
            var newUser = FbUser.Builder("authorized").Name("authorized").Build();
            await _client.IdentifyAsync(newUser);

            // show user window
            new UserWindow().Show();
        }
    }
}