using System.Threading.Tasks;

namespace FeatBit.Sdk.Client.DataSynchronizer
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