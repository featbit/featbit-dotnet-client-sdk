using FeatBit.ClientSdk.Concurrent;
using FeatBit.ClientSdk.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public bool Initialized => throw new NotImplementedException();
        private string _remoteServerUrl = "";
        private int _pollingInterval = 30000;
        private readonly FbOptions _options;
        private readonly ILogger _logger;

        public FbClient(FbOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<FbClient>();
        }

        public FbClient()
        {
        }

        #region initialization methods
        public void Init(string remoteServerUrl, bool enablePooling = false, int poolingInterval = 30000)
        {
            _remoteServerUrl = remoteServerUrl;
            if (enablePooling == true)
                _pollingInterval = poolingInterval;
        }

        public async Task InitAsync(string remoteServerUrl, bool enablePooling = false, int poolingInterval = 30000)
        {
            this.Init(remoteServerUrl, enablePooling, poolingInterval);
            LoadLatestCollection(await RetriveFeatureFlagsFromServerByHttpAPIAsync());
        }

        public void LoadLatestCollection(List<FeatureFlag> featureFlags)
        {
            FeatureFlagsCollection.Instance.InitOrUpdateCollection(featureFlags);
        }

        public void SaveToLocal(Action<List<FeatureFlag>> action)
        {
            action(FeatureFlagsCollection.Instance.GetAllLatestFeatureFlags());
        }

        private async Task<List<FeatureFlag>> RetriveFeatureFlagsFromServerByHttpAPIAsync()
        {
            var url = $"{_remoteServerUrl}/api/public/sdk/client/latest-all";
            var keyId = "bot-id";
            var name = "bot";
            var requestBody = new
            {
                keyId = keyId,
                name = name,
                customizedProperties = new[]
                {
                    new { name = "level", value = "high" },
                    new { name = "localtion", value = "us" }
                }
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "RQrHZX7ClUmjS56c5aq_Mw3zSJhPUPg0mRr59x9t_NSg");
                httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response received successfully:");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<FeatureFlag>>(responseContent) ?? new List<FeatureFlag>();
                }
                else
                {
                    throw new Exception("Failed to retrieve feature flags from server");
                }
            }
        }

        #endregion

        #region evaluation methods
        public bool BoolVariation(string key, bool defaultValue = false)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection.Instance.TryGetValue(key, out ff) == true)
            {
                if(ff.VariationType.ToLower() == "boolean")
                {
                    return ff.Variation == "true";
                }
                else
                {
                    throw new Exception("Variation type is not boolean");
                }
            }
            else
            {
                FeatureFlagsCollection.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "boolean"
                });
                return defaultValue;
            }
        }

        public double DoubleVariation(string key, double defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "number")
                {
                    return Convert.ToDouble(ff.Variation);
                }
                else
                {
                    throw new Exception("Variation type is not Double");
                }
            }
            else
            {
                FeatureFlagsCollection.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "number"
                });
                return defaultValue;
            }
        }

        public float FloatVariation(string key, float defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "number")
                {
                    return float.Parse(ff.Variation.ToLower(), CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    throw new Exception("Variation type is not Float");
                }
            }
            else
            {
                FeatureFlagsCollection.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "number"
                });
                return defaultValue;
            }
        }

        public T ObjectVariation<T>(string key, T defaultValue = default)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "string")
                {
                    return JsonSerializer.Deserialize<T>(ff.Variation) ?? defaultValue;
                }
                else
                {
                    throw new Exception("Variation type is not Json");
                }
            }
            else
            {
                FeatureFlagsCollection.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = JsonSerializer.Serialize(defaultValue),
                    VariationType = "string"
                });
                return defaultValue;
            }
        }

        public int IntVariation(string key, int defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "number")
                {
                    return Convert.ToInt32(ff.Variation);
                }
                else
                {
                    throw new Exception("Variation type is not Number");
                }
            }
            else
            {
                FeatureFlagsCollection.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "number"
                });
                return defaultValue;
            }
        }

        public string StringVariation(string key, string defaultValue = "")
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "string")
                {
                    return ff.Variation;
                }
                else
                {
                    throw new Exception("Variation type is not String");
                }
            }
            else
            {
                FeatureFlagsCollection.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString(),
                    VariationType = "string"
                });
                return defaultValue;
            }
        }

        #endregion

        public void Track(FbIdentity user, string eventName)
        {
            throw new NotImplementedException();
        }

        public void Track(FbIdentity user, string eventName, double metricValue)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public bool FlushAndWait(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
