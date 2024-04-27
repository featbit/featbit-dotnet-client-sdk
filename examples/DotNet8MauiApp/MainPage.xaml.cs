using FeatBit.ClientSdk;
using FeatBit.ClientSdk.Events;

namespace DotNet8MauiApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IFbClient _fbClient;
        int count = 0;

        public MainPage(MainViewModel viewModel, IFbClient fbClient)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _fbClient = fbClient;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object? sender, EventArgs e)
        {
            Label_Welcome.IsVisible = _fbClient.BoolVariation("is-welcome-text-visible", false);
        }

        private void FeatureFlagsUpdated(object? sender, FeatureFlagsUpdatedEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                Label_Welcome.IsVisible = _fbClient.BoolVariation("is-welcome-text-visible", false);
            });
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
