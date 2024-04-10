﻿using System;
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
        bool BoolVariation(string key, bool defaultValue = false);
        int IntVariation(string key, int defaultValue = 0);
        float FloatVariation(string key, float defaultValue = 0);
        double DoubleVariation(string key, double defaultValue = 0);
        string StringVariation(string key, string defaultValue = "");
        void Dispose();
        Task DisposeAsync();
    }
}
