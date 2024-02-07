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
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Linq;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public bool Initialized => throw new NotImplementedException();
        private readonly FbOptions _options;
        private readonly ILogger _logger;
        private FbIdentity _identity;

        public FbClient(FbOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<FbClient>();
        }

        public FbClient()
        {
        }

        public void Identify(FbIdentity identity)
        {
            _identity = identity;
        }

        #region initialization methods
        public void LoadLatestCollectionFromRemoteServer()
        {
        }

        public async Task LoadLatestCollectionFromRemoteServerAsync()
        {
            LoadLatestCollection(await RetriveFeatureFlagsFromServerByHttpAPIAsync());
        }

        public void LoadLatestCollection(List<FeatureFlag> featureFlags)
        {
            FeatureFlagsCollection.Instance.InitOrUpdateCollection(featureFlags);
        }

        public async Task LoadLocalCollectionAsync(Func<Task<List<FeatureFlag>>> loadActionAsync)
        {
            var featureFlags = await loadActionAsync();
            LoadLatestCollection(featureFlags);
        }

        public void SaveToLocal(Action<List<FeatureFlag>> action)
        {
            action(FeatureFlagsCollection.Instance.GetAllLatestFeatureFlags());
        }

        private async Task<List<FeatureFlag>> RetriveFeatureFlagsFromServerByHttpAPIAsync()
        {
            var url = $"{_options.EventUri}api/public/sdk/client/latest-all";
            var requestBody = new
            {
                keyId = _identity.Key,
                name = _identity.Name,
                customizedProperties = _identity.Custom.ToArray()
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", _options.EnvSecret);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var contentStr = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(contentStr, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

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
