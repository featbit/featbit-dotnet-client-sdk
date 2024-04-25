using System.Text.Json.Serialization;

namespace FeatBit.ClientSdk.Model
{
    public class FeatureFlag
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("variation")] public string Variation { get; set; }
        [JsonPropertyName("variationType")] public string VariationType { get; set; }

        [JsonPropertyName("variationId")] public string VariationId { get; set; }

        [JsonPropertyName("sendToExperiment")] public bool SendToExperiment { get; set; }

        public FeatureFlag()
        {
        }

        public FeatureFlag(
            string id,
            string variation,
            string variationType,
            string variationId,
            bool sendToExperiment)
        {
            Id = id;
            Variation = variation;
            VariationType = variationType;
            VariationId = variationId;
            SendToExperiment = sendToExperiment;
        }
    }
}