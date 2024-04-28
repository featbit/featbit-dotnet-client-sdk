using System;
using System.Collections.Generic;
using FeatBit.Sdk.Client.Model;

namespace FeatBit.Sdk.Client.Events
{
    // TODO: delete this
    public class FeatureFlagsUpdatedEventArgs : EventArgs
    {
        public List<FeatureFlag> UpdatedFeatureFlags { get; set; }
    }
}
