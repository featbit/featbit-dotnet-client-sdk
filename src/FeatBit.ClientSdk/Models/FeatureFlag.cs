using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FeatBit.ClientSdk
{
    public class FeatureFlag
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("variation")]
        public string Variation { get; set; }
        [JsonPropertyName("variationType")]
        public string VariationType { get; set; }

        public FeatureFlag()
        {

        }

        public FeatureFlag(
            string id,
            string variation,
            string variationType)
        {
            Id = id;
            Variation = variation;
            VariationType = variationType;
        }
    }
}