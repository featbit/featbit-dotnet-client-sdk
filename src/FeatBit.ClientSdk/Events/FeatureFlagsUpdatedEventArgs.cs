﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FeatBit.ClientSdk
{
    public class FeatureFlagsUpdatedEventArgs : EventArgs
    {
        public List<FeatureFlag> UpdatedFeatureFlags { get; set; }
    }
}
