namespace FeatBit.ClientSdk.Model
{
    public class FeatureFlag
    {
        public string Id { get; set; }

        public string Variation { get; set; }

        public string VariationType { get; set; }

        public string VariationId { get; set; }

        public bool SendToExperiment { get; set; }

        public string MatchReason { get; set; }
    }
}