using System.Threading.Tasks;

namespace FeatBit.ClientSdk.DataSynchronizer
{
    internal sealed class NullDataSynchronizer : IDataSynchronizer
    {
        public bool Initialized => true;

        public Task<bool> StartAsync()
        {
            return Task.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}