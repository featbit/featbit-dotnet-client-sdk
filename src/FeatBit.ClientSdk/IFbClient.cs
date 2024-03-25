using FeatBit.ClientSdk.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public interface IFbClient
    {
        /// <summary>
        /// Indicates whether the client is ready to be used.
        /// </summary>
        /// <value>true if the client is ready</value>
        bool Initialized { get; }
        event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;
        void StartTimer();
        Task<List<FeatureFlag>> GetAndUpdateToLatestAllAsync();
        void Identify(FbUser identity);
        void Logout();
        void SaveToLocal(Action<Dictionary<string, FeatureFlag>> action);
        bool BoolVariation(string key, bool defaultValue = false);
        int IntVariation(string key, int defaultValue = 0);
        float FloatVariation(string key, float defaultValue = 0);
        double DoubleVariation(string key, double defaultValue = 0);
        string StringVariation(string key, string defaultValue = "");
        T ObjectVariation<T>(string key, T defaultValue = default);
        void Track(FbUser user, string eventName);
        void Track(FbUser user, string eventName, double metricValue);
        void Flush();
        bool FlushAndWait(TimeSpan timeout);
        Task CloseAsync();
    }
}
