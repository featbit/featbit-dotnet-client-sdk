using FeatBit.ClientSdk.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public interface IFbClient
    {
        event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;
        void StartAutoDataSync();
        void StopAutoDataSync();
        List<FeatureFlag> GetLatestAll();
        void Identify(FbUser identity);
        Task IdentifyAsync(FbUser fbUser, bool autoSync = false);
        void Logout();
        void InitFeatureFlagsFromLocal(List<FeatureFlag> featureFlags, bool autoSync = false);
        bool BoolVariation(string key, bool defaultValue = false);
        int IntVariation(string key, int defaultValue = 0);
        float FloatVariation(string key, float defaultValue = 0);
        double DoubleVariation(string key, double defaultValue = 0);
        string StringVariation(string key, string defaultValue = "");
        void Track(FbUser user, string eventName);
        void Track(FbUser user, string eventName, double metricValue);
        void Flush();
        bool FlushAndWait(TimeSpan timeout);
        Task CloseAsync();
    }
}
