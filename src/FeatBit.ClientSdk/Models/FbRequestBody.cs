using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FeatBit.ClientSdk.Models
{
    public class FbRequestBody
    {
        [JsonPropertyName("keyId")]
        public string KeyId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

    }
}
