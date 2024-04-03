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
using FeatBit.ClientSdk.Models;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;

        private readonly FbOptions _options;
        private readonly ILogger _logger;

        internal readonly IDataSynchronizer _dataSynchronizer;
        internal readonly IInsightsAndEventSenderService _insightsAndEventSenderService;

        private readonly ConcurrentDictionary<String, FeatureFlag> _featureFlagsCollection;
        private readonly ApplicationTypeEnum _appType;
        private readonly DataSyncMethodEnum _dataSyncMethod;

        private FbUser _fbUser;

        public FbClient(FbOptions options, FbUser fbUser = null, bool autoSync = true, ApplicationTypeEnum applicatoinType = ApplicationTypeEnum.Standard)
        {
            _options = options;
            _logger = _options.LoggerFactory.CreateLogger<FbClient>();
            _dataSyncMethod = _options.DataSyncMethod;
            _featureFlagsCollection = new ConcurrentDictionary<string, FeatureFlag>();
            _dataSynchronizer = new PollingDataSynchronizer(_options, _featureFlagsCollection);
            _appType = applicatoinType;
            _insightsAndEventSenderService = new InsightsAndEventSenderService(_options);

            GenerateDefaultUser(fbUser);

            if (autoSync == true)
                StartAutoDataSync();
        }

        public void StartAutoDataSync()
        {
            if(_dataSyncMethod == DataSyncMethodEnum.Polling)
            {
                Task.Run(async () => {
                    _dataSynchronizer.FeatureFlagsUpdated += DataSynchronizer_FeatureFlagsUpdated;
                    await _dataSynchronizer.StartAsync();
                });
            }
        }

        public void StopAutoDataSync()
        {
            if (_dataSyncMethod == DataSyncMethodEnum.Polling)
            {
                Task.Run(async () =>
                {
                    _dataSynchronizer.FeatureFlagsUpdated -= DataSynchronizer_FeatureFlagsUpdated;
                    await _dataSynchronizer.StopAsync();
                }).Wait();
            }
        }

        public void GenerateDefaultUser(FbUser fbUser)
        {
            if (fbUser == null)
            {
                var randomKey = "unlogin-" + Guid.NewGuid().ToString();
                _fbUser = FbUser.Builder(randomKey)
                    .Name(randomKey)
                    .Custom("application-type", _appType.ToString())
                    .Build();
            }
            else
            {
                _fbUser = fbUser.ShallowCopy();
            }
        }

        public void Identify(FbUser fbUser)
        {
            _fbUser = fbUser.ShallowCopy();
            _dataSynchronizer.Identify(fbUser);
        }

        public async Task IdentifyAsync(FbUser fbUser, bool autoSync = false)
        {
            StopAutoDataSync();
            _fbUser = fbUser.ShallowCopy();
            _dataSynchronizer.Identify(fbUser);
            await _dataSynchronizer.UpdateFeatureFlagCollectionAsync();
            if (autoSync == true)
                StartAutoDataSync();
        }

        public void InitFeatureFlagsFromLocal(List<FeatureFlag> featureFlags, bool autoSync = false)
        {
            StopAutoDataSync();
            _dataSynchronizer.UpdateFeatureFlagsCollection(featureFlags);
            if (autoSync == true)
                StartAutoDataSync();
        }

        public List<FeatureFlag> GetLatestAll()
        {
            var ffs = new List<FeatureFlag>();
            foreach (var item in _featureFlagsCollection)
            {
                ffs.Add(item.Value.ShallowCopy());
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
            return GetFeatureFlagValue<bool>(key, defaultValue, "boolean");
        }

        public double DoubleVariation(string key, double defaultValue = 0)
        {
            return GetFeatureFlagValue<double>(key, defaultValue, "number");
        }

        public float FloatVariation(string key, float defaultValue = 0)
        {
            return GetFeatureFlagValue<float>(key, defaultValue, "number");
        }

        public T ObjectVariation<T>(string key, T defaultValue = default)
        {
            return GetFeatureFlagValue<T>(key, defaultValue, "string");
        }

        public int IntVariation(string key, int defaultValue = 0)
        {
            return GetFeatureFlagValue<int>(key, defaultValue, "number");
        }

        public string StringVariation(string key, string defaultValue = "")
        {
            return GetFeatureFlagValue<string>(key, defaultValue, "string");
        }

        private T GetFeatureFlagValue<T>(string key, T defaultValue, string variationType)
        {
            var returnValue = defaultValue;
            FeatureFlag ff = new FeatureFlag();
            if (_featureFlagsCollection.TryGetValue(key, out ff))
            {
                if (ff.VariationType.ToLower() == variationType.ToLower())
                {
                    returnValue = ConvertValue<T>(ff.Variation);
                }
                else
                {
                    _logger.LogError($"Variation type is not {variationType}");
                }
            }
            Task.Run(async () => await _insightsAndEventSenderService.TrackInsightAsync(
                    ComposeToVariationInsight(key, returnValue), _fbUser));
            return returnValue;
        }

        private T ConvertValue<T>(string value)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)(object)(value.ToLower() == "true");
            }
            else if (typeof(T) == typeof(double))
            {
                return (T)(object)Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)float.Parse(value.ToLower(), CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            else if (typeof(T) == typeof(string))
            {
                return (T)(object)value;
            }
            else
            {
                throw new Exception($"Unsupported variation type: {typeof(T)}");
            }
        }

        private VariationInsight ComposeToVariationInsight<T>(string key, T defaultValue)
        {
            return new VariationInsight
            {
                FeatureFlagKey = key,
                SendToExperiment = false,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Variation = new Variation(key, Convert.ToString(defaultValue, CultureInfo.InvariantCulture))
            };
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
            _dataSynchronizer.FeatureFlagsUpdated -= DataSynchronizer_FeatureFlagsUpdated;
            await _dataSynchronizer.StopAsync();
            _logger.LogInformation("FbClient successfully closed.");
        }
    }
}
