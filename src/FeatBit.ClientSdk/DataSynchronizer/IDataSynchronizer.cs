using FeatBit.ClientSdk.Events;
using System;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public interface IDataSynchronizer
    {
        /// <summary>
        /// Indicates whether the data synchronizer has finished initializing.
        /// </summary>
        public bool Initialized { get; }

        event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;

        void Identify(FbUser fbUser);

        Task StartAsync();

        Task StopAsync();
    }
}
