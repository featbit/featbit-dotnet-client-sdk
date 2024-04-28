using System;

namespace FeatBit.Sdk.Client.Model
{
    internal sealed class Insight
    {
        public object User { get; set; }

        public VariationInsight[] Variations { get; set; }

        public Insight(FbUser user, FeatureFlag flag)
        {
            User = user.AsEndUser();
            Variations = new VariationInsight[1];
            Variations[0] = new VariationInsight(flag);
        }
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
}