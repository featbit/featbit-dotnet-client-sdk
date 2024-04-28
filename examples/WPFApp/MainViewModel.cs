using System.ComponentModel;
using System.Windows.Input;
using FeatBit.Sdk.Client;
using FeatBit.Sdk.Client.Events;

namespace WPFApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IFbClient _fbClient;

        public MainViewModel(IFbClient fbClient)
        {
            _fbClient = fbClient;
            SetVisibilities();

            GetLatestFeatureFlagsValuesCommand = new RelayCommand(GetLatestFeatureFlagsValues, CanExecuteGetLatestFeatureFlagsValues);
        }

        private void FeatureFlagsUpdated(object? sender, FeatureFlagsUpdatedEventArgs e)
        {
            SetVisibilities();
        }

        private void SetVisibilities()
        {
            WelcomeTextVisibility = _fbClient.StringVariation("welcome-text-visibility", "Collapsed");
        }

        #region Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        private string _welcomeText = "Welcome!";
        public string WelcomeText
        {
            get => _welcomeText;
            set
            {
                _welcomeText = value;
                OnPropertyChanged(nameof(WelcomeText));
            }
        }

        private string _welcomeTextVisibility;
        public string WelcomeTextVisibility
        {
            get => _welcomeTextVisibility;
            set
            {
                _welcomeTextVisibility = value;
                OnPropertyChanged(nameof(WelcomeTextVisibility));
            }
        }
        #endregion


        #region Commands

        public ICommand GetLatestFeatureFlagsValuesCommand { get; }
        private bool CanExecuteGetLatestFeatureFlagsValues()
        {
            return true;
        }
        private async void GetLatestFeatureFlagsValues()
        {
            // TODO: Implement this method
            // await _fbClient.UpdateToLatestAsync();
            SetVisibilities();
        }
        #endregion
    }
}
