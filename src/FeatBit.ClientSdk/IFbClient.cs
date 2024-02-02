using FeatBit.ClientSdk.Models;
using System;
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

        bool BoolVariation(string key, bool defaultValue = false);
        int IntVariation(string key, int defaultValue = 0);
        float FloatVariation(string key, float defaultValue = 0);
        double DoubleVariation(string key, double defaultValue = 0);
        string StringVariation(string key, string defaultValue = "");
        public object? ObjectVariation(string key, object defaultValue = null);
        void Track(FbIdentity user, string eventName);
        void Track(FbIdentity user, string eventName, double metricValue);
        void Flush();
        bool FlushAndWait(TimeSpan timeout);
        Task CloseAsync();
    }
}
