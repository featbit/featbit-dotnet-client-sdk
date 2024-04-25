using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Events;
using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk.DataSynchronizer
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
