using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Enums;
using Microsoft.Extensions.Logging;

namespace FeatBit.ClientSdk
{
    public interface IFbClient
    {
        event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;
        void InitLoggerForWebAssembly(ILoggerFactory loggerFactory);
        void StartAutoData();
        void StopAutoData();
        List<FeatureFlag> GetLatestAll();
        void Identify(FbUser identity);
        Task IdentifyAsync(FbUser fbUser);
        Task UpdateToLatestAsync();
        void Logout();
        void InitFeatureFlagsFromLocal(List<FeatureFlag> featureFlags);
        bool BoolVariation(string key, bool defaultValue = false, bool trackInsight = true);
        int IntVariation(string key, int defaultValue = 0, bool trackInsight = true);
        float FloatVariation(string key, float defaultValue = 0, bool trackInsight = true);
        double DoubleVariation(string key, double defaultValue = 0, bool trackInsight = true);
        string StringVariation(string key, string defaultValue = "", bool trackInsight = true);
        void Track(FbUser user, string eventName);
        void Track(FbUser user, string eventName, double metricValue);
        void Dispose();
        Task DisposeAsync();
    }
}
