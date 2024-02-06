﻿using FeatBit.ClientSdk.Models;
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

        void Init(string remoteServerUrl, bool enablePooling = false, int poolingInterval = 30000);
        Task InitAsync(string remoteServerUrl, bool enablePooling = false, int poolingInterval = 30000);
        void LoadLatestCollection(List<FeatureFlag> featureFlags);

        bool BoolVariation(string key, bool defaultValue = false);
        int IntVariation(string key, int defaultValue = 0);
        float FloatVariation(string key, float defaultValue = 0);
        double DoubleVariation(string key, double defaultValue = 0);
        string StringVariation(string key, string defaultValue = "");
        T ObjectVariation<T>(string key, T defaultValue = default);
        void Track(FbIdentity user, string eventName);
        void Track(FbIdentity user, string eventName, double metricValue);
        void Flush();
        bool FlushAndWait(TimeSpan timeout);
        Task CloseAsync();
    }
}
