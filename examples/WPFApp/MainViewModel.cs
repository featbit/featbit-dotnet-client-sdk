using FeatBit.ClientSdk;
using System.ComponentModel;
using System.Windows.Input;

namespace WPFApp
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IFbClient _fbClient;

        public MainViewModel(IFbClient fbClient)
        {
            _fbClient = fbClient;

            MyCommand = new RelayCommand(ExecuteMyCommand, CanExecuteMyCommand);
        }

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

        #region Commands
        // MyCommand
        public ICommand MyCommand { get; }
        private bool CanExecuteMyCommand()
        {
            return true;
        }
        private void ExecuteMyCommand()
        {
            SomeProperty = $"Random Generate Guid {Guid.NewGuid().ToString()}";
        }
        #endregion
    }
}
