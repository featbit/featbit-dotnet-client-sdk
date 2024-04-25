using System;
using System.Collections.Generic;

namespace FeatBit.ClientSdk.Model
{
    public sealed class Insight
    {
        public EndUser User { get; set; }

        public IEnumerable<VariationInsight> Variations { get; set; } = Array.Empty<VariationInsight>();

        public IEnumerable<MetricInsight> Metrics { get; set; } = Array.Empty<MetricInsight>();
    }

    public class EndUser
    {
        public string KeyId { get; set; }

        public string Name { get; set; }

        public CustomizedProperty[] CustomizedProperties { get; set; } = Array.Empty<CustomizedProperty>();
    }

    public class CustomizedProperty
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class EndUserConsts
    {
        public const string KeyId = "keyId";

        public const string Name = "name";
    }

    public class VariationInsight
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

    public class Variation
    {
        public string Id { get; set; }

        public string Value { get; set; }

        public Variation(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }

    public class MetricInsight
    {
        public string UserId { get; set; }
        public string EventName { get; set; }

        public double NumericValue { get; set; }
        public long Timestamp { get; set; }
    }
}