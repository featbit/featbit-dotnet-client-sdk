using FeatBit.ClientSdk;
using System.ComponentModel;
using System.Windows.Input;

namespace WPFApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        
        #region Properties
        private string _someProperty;
        public string SomeProperty
        {
            get => _someProperty;
            set
            {
                _someProperty = value;
                OnPropertyChanged(nameof(SomeProperty));
            }
        }
        #endregion

        public ICommand MyCommand { get; }
        IFbClient _fbClient;

        public MainViewModel(IFbClient fbClient)
        {
            _fbClient = fbClient;

            MyCommand = new RelayCommand(ExecuteMyCommand, CanExecuteMyCommand);
        }

        private bool CanExecuteMyCommand()
        {
            // Logic to determine if command can execute
            return true; // For simplicity, always true here
        }
        private void ExecuteMyCommand()
        {
            SomeProperty = $"Random Generate Guid {Guid.NewGuid().ToString()}";
        }
    }
}
