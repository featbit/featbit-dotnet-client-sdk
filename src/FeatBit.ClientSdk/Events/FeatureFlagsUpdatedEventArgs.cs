using System;
using System.Collections.Generic;
using FeatBit.ClientSdk.Model;

namespace FeatBit.ClientSdk.Events
{
    // TODO: delete this
    public class FeatureFlagsUpdatedEventArgs : EventArgs
    {
        public List<FeatureFlag> UpdatedFeatureFlags { get; set; }
    }
}
