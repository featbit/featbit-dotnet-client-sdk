using FeatBit.ClientSdk.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public interface IDataSynchronizer
    {
        event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;

        void Identify(FbUser fbUser);

        Task StartAsync();

        Task StopAsync();

        Task UpdateFeatureFlagCollectionAsync(FbUser newFbUser = null);
        void UpdateFeatureFlagsCollection(List<FeatureFlag> ffs);
    }
}
