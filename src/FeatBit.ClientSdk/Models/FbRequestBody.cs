using System.Text.Json.Serialization;

namespace FeatBit.ClientSdk
{
    public class FbRequestBody
    {
        [JsonPropertyName("keyId")]
        public string KeyId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

    }
}
