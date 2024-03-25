using FeatBit.ClientSdk.Enums;
using FeatBit.ClientSdk.Events;
using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public bool Initialized => _dataSynchronizer.Initialized;
        public event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;

        private readonly FbOptions _options;
        private readonly ILogger _logger;
        private FbUser _fbUser;
        private FbUser _unloginUser;
        internal readonly IDataSynchronizer _dataSynchronizer;
        private readonly ConcurrentDictionary<String, FeatureFlag> _featureFlagsCollection;
        private readonly IFeatBitRestfulService _apiService;
        private readonly ApplicationTypeEnum _appType;

        public FbClient(FbOptions options, bool autoSync = true, ApplicationTypeEnum applicatoinType = ApplicationTypeEnum.Standard)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<FbClient>();
            _featureFlagsCollection = new ConcurrentDictionary<string, FeatureFlag>();
            _dataSynchronizer = new PollingDataSynchronizer(options, _featureFlagsCollection);
            _apiService = new FeatBitRestfulService(options);
            _appType = applicatoinType;
            GenerateDefaultUser();

            if (autoSync == true)
                StartTimer();
        }

        public void StartTimer()
        {
            Task.Run(async () => await StartDataSyncAsync());
        }

        private async Task StartDataSyncAsync()
        {
            _dataSynchronizer.Identify(_fbUser);
            _dataSynchronizer.FeatureFlagsUpdated += DataSynchronizer_FeatureFlagsUpdated;
            await _dataSynchronizer.StartAsync();
        }

        public void GenerateDefaultUser()
        {
            var randomKey = "random-" + Guid.NewGuid().ToString();
            _unloginUser = FbUser.Builder(randomKey)
                .Name(randomKey)
                .Custom("application-type", _appType.ToString())
                .Build();
            _fbUser = _unloginUser.ShallowCopy();
        }

        public void Identify(FbUser fbUser)
        {
            _fbUser = fbUser.ShallowCopy();
            _dataSynchronizer.Identify(fbUser);
        }

        public async Task<List<FeatureFlag>> GetAndUpdateToLatestAllAsync()
        {
            var ffs = await _apiService.GetLatestAllAsync(_fbUser);
            _featureFlagsCollection.Clear();
            foreach (var item in ffs)
            {
                _featureFlagsCollection.TryAdd(item.Id, item.ShallowCopy());
            }
            return ffs;
        }

        private void DataSynchronizer_FeatureFlagsUpdated(object sender, FeatureFlagsUpdatedEventArgs e)
        {
            FeatureFlagsUpdated?.Invoke(sender, e);
        }

        public void Logout()
        {
            _fbUser = null;
        }

        #region initialization methods
        public void SaveToLocal(Action<Dictionary<string, FeatureFlag>> action)
        {
            var newDic = _featureFlagsCollection.ToDictionary(x => x.Key, x => x.Value.ShallowCopy());
            action(newDic);
        }
        #endregion


        #region utils
        private FeatureFlag ComposeNewFeatureFlagValue(string key, string value, string type)
        {
            return new FeatureFlag
            {
                Id = key,
                Variation = value,
                VariationType = type
            };
        }   
        private void UpdateFeatureFlagNewValueToCollection(string key, string value, string type)
        {
            var ffNewValue = ComposeNewFeatureFlagValue(key, value, type);
            _featureFlagsCollection.AddOrUpdate(key, ffNewValue, (existingKey, existingValue) => ffNewValue);
        }
        #endregion

        #region evaluation methods
        public bool BoolVariation(string key, bool defaultValue = false)
        {
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff) == true)
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
                UpdateFeatureFlagNewValueToCollection(key, defaultValue.ToString().ToLower(), "boolean");
                return defaultValue;
            }
        }

        public double DoubleVariation(string key, double defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() != "number")
                {
                    throw new Exception("Variation type is not Double");
                }
                return Convert.ToDouble(ff.Variation);
            }
            else
            {
                UpdateFeatureFlagNewValueToCollection(key, defaultValue.ToString().ToLower(), "number");
                return defaultValue;
            }
        }

        public float FloatVariation(string key, float defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() != "number")
                {
                    throw new Exception("Variation type is not Float");
                }
                return float.Parse(ff.Variation.ToLower(), CultureInfo.InvariantCulture.NumberFormat);
            }
            else
            {
                UpdateFeatureFlagNewValueToCollection(key, defaultValue.ToString().ToLower(), "number");
                return defaultValue;
            }
        }

        public T ObjectVariation<T>(string key, T defaultValue = default)
        {
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() != "string")
                {
                    throw new Exception("Variation type is not Json");
                }
                return JsonSerializer.Deserialize<T>(ff.Variation) ?? defaultValue;
            }
            else
            {
                UpdateFeatureFlagNewValueToCollection(key, JsonSerializer.Serialize(defaultValue), "string");
                return defaultValue;
            }
        }

        public int IntVariation(string key, int defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() != "number")
                {
                    throw new Exception("Variation type is not Number");
                }
                return Convert.ToInt32(ff.Variation);
            }
            else
            {
                UpdateFeatureFlagNewValueToCollection(key, defaultValue.ToString().ToLower(), "number");
                return defaultValue;
            }
        }

        public string StringVariation(string key, string defaultValue = "")
        {
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() != "string")
                {
                    throw new Exception("Variation type is not String");
                }
                return ff.Variation;
            }
            else
            {
                UpdateFeatureFlagNewValueToCollection(key, defaultValue.ToString(), "string");
                return defaultValue;
            }
        }

        #endregion

        public void Track(FbUser user, string eventName)
        {
            throw new NotImplementedException();
        }

        public void Track(FbUser user, string eventName, double metricValue)
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
        public async Task CloseAsync()
        {
            _logger.LogInformation("Closing FbClient...");
            await _dataSynchronizer.StopAsync();
            //_eventProcessor.FlushAndClose(_options.FlushTimeout);
            _logger.LogInformation("FbClient successfully closed.");
        }
    }
}
