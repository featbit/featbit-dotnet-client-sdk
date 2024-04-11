using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FeatBit.ClientSdk;


namespace DotNet8MauiApp
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IFbClient _fbClient;

        public MainViewModel(IFbClient fbClient)
        {
            _fbClient = fbClient;
        }

        #region Observable Properties
        #endregion

        [RelayCommand]
        public async Task SimulateAsyncIdentify()
        {
            await _fbClient.IdentifyAsync(FbUser.Builder("simulation-net-maui")
                                                .Name("Simulation Net Maui")
                                                .Custom("mauiVersion", "8")
                                                .Build());
        }

        [RelayCommand]
        public async Task UpdateToLatestFeatureFlagsValue()
        {
            await _fbClient.UpdateToLatestAsync();
        }
    }
}
