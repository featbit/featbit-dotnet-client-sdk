using System;
using System.Collections.Generic;

namespace FeatBit.ClientSdk.Model
{
    internal sealed class Insight
    {
        public EndUser User { get; set; }

        public IEnumerable<VariationInsight> Variations { get; set; } = Array.Empty<VariationInsight>();

        public IEnumerable<MetricInsight> Metrics { get; set; } = Array.Empty<MetricInsight>();
    }

    internal sealed class EndUser
    {
        public string KeyId { get; set; }

        public string Name { get; set; }

        public CustomizedProperty[] CustomizedProperties { get; set; } = Array.Empty<CustomizedProperty>();
    }

    internal sealed class CustomizedProperty
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    internal sealed class VariationInsight
    {
        public string FeatureFlagKey { get; set; }

        public Variation Variation { get; set; }

        public bool SendToExperiment { get; set; }

        public long Timestamp { get; set; }

        public VariationInsight(FeatureFlag ff)
        {
            FeatureFlagKey = ff.Id;
            Variation = new Variation(ff.VariationId, ff.Variation);
            SendToExperiment = ff.SendToExperiment;
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    internal sealed class Variation
    {
        public string Id { get; set; }

        public string Value { get; set; }

        public Variation(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }

    internal sealed class MetricInsight
    {
        public string UserId { get; set; }
        public string EventName { get; set; }

        public double NumericValue { get; set; }
        public long Timestamp { get; set; }
    }
}