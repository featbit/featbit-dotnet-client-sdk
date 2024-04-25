using System;
using System.Collections.Generic;
using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk.Events
{
    public class FeatureFlagsUpdatedEventArgs : EventArgs
    {
        public List<FeatureFlag> UpdatedFeatureFlags { get; set; }
    }
}
