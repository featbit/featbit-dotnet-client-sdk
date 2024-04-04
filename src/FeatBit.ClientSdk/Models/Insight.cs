using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FeatBit.ClientSdk.Models
{
    public sealed class Insight
    {
        public EndUser? User { get; set; }

        public IEnumerable<VariationInsight> Variations { get; set; } = Array.Empty<VariationInsight>();

        public IEnumerable<MetricInsight> Metrics { get; set; } = Array.Empty<MetricInsight>();
    }

    public class EndUser
    {
        public string KeyId { get; set; }

        public string Name { get; set; }

        public CustomizedProperty[] CustomizedProperties { get; set; } = Array.Empty<CustomizedProperty>();

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(KeyId);
        }

        public string ValueOf(string property)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                return string.Empty;
            }

            if (property == EndUserConsts.KeyId)
            {
                return KeyId;
            }

            if (property == EndUserConsts.Name)
            {
                return Name;
            }

            var value = CustomizedProperties?.FirstOrDefault(x => x.Name == property);
            return value?.Value ?? string.Empty;
        }
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

        public VariationInsight ShallowCopy()
        {
            var newVI = this.MemberwiseClone() as VariationInsight;
            newVI.Variation = new Variation(Variation.Id, Variation.Value);
            return newVI;
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

        public static readonly Variation Empty = new(string.Empty, string.Empty);
    }
    public class MetricInsight
    {
        public string Route { get; set; }

        public string Type { get; set; }

        public string EventName { get; set; }

        public float NumericValue { get; set; }

        public string AppType { get; set; }

        public long Timestamp { get; set; }
    }
}
