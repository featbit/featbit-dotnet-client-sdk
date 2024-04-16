using FeatBit.ClientSdk.Enums;
using FeatBit.ClientSdk.Models;
using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;

        private readonly FbOptions _options;
        private ILogger _logger;

        internal readonly IDataSynchronizer _dataSynchronizer;
        internal readonly IInsightsAndEventSenderService _insightsAndEventSenderService;

        private readonly ConcurrentDictionary<String, FeatureFlag> _featureFlagsCollection;
        private readonly ApplicationTypeEnum _appType;
        private readonly DataSyncMethodEnum _dataSyncMethod;

        private FbUser _fbUser;

        public FbClient(FbOptions options, FbUser fbUser = null, bool autoSync = true, ApplicationTypeEnum applicatoinType = ApplicationTypeEnum.Standard)
        {
            _options = options;
            if (_options.LoggerFactory != null)
                _logger = _options.LoggerFactory.CreateLogger<FbClient>();
            _dataSyncMethod = _options.DataSyncMethod;
            _featureFlagsCollection = new ConcurrentDictionary<string, FeatureFlag>();
            _appType = applicatoinType;
            _insightsAndEventSenderService = new InsightsAndEventSenderService(_options);

            GenerateDefaultUser(fbUser);

            _dataSynchronizer = new PollingDataSynchronizer(_options, _featureFlagsCollection);
            _dataSynchronizer.Identify(_fbUser);

            if (autoSync == true)
                StartAutoData();
        }

        public void InitLoggerForWebAssembly(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FbClient>();
        }

        public void StartAutoData()
        {
            if(_dataSyncMethod == DataSyncMethodEnum.Polling)
            {
                Task.Run(async () => {
                    _dataSynchronizer.FeatureFlagsUpdated += DataSynchronizer_FeatureFlagsUpdated;
                    await _dataSynchronizer.StartAsync();
                });
            }
        }

        public void StopAutoData()
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
                var randomKey = "client-sdk-" + Guid.NewGuid().ToString();
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

        public async Task IdentifyAsync(FbUser fbUser)
        {
            _fbUser = fbUser.ShallowCopy();
            await _dataSynchronizer.UpdateFeatureFlagCollectionAsync(fbUser.ShallowCopy());
        }

        public async Task UpdateToLatestAsync()
        {
            await _dataSynchronizer.UpdateFeatureFlagCollectionAsync();
        }

        public void InitFeatureFlagsFromLocal(List<FeatureFlag> featureFlags)
        {
            _dataSynchronizer.UpdateFeatureFlagsCollection(featureFlags);
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
        #endregion

        #region evaluation methods
        public bool BoolVariation(string key, bool defaultValue = false, bool trackInsight = true)
            => GetFeatureFlagValue(key, defaultValue, ValueConverters.Bool);

        public double DoubleVariation(string key, double defaultValue = 0, bool trackInsight = true)
            => GetFeatureFlagValue(key, defaultValue, ValueConverters.Double);

        public float FloatVariation(string key, float defaultValue = 0, bool trackInsight = true)
            => GetFeatureFlagValue(key, defaultValue, ValueConverters.Float);

        public int IntVariation(string key, int defaultValue = 0, bool trackInsight = true) 
            => GetFeatureFlagValue(key, defaultValue, ValueConverters.Int);

        public string StringVariation(string key, string defaultValue = "", bool trackInsight = true)
            => GetFeatureFlagValue(key, defaultValue, ValueConverters.String);

        private TValue GetFeatureFlagValue<TValue>(
                    string key,
                    TValue defaultValue,
                    ValueConverter<TValue> converter,
                    bool trackInsight = true)
        {
            _featureFlagsCollection.TryGetValue(key, out FeatureFlag ff);
            if (ff != null && trackInsight == true)
            {
                Task.Run(async () => await _insightsAndEventSenderService.TrackInsightAsync(new VariationInsight(ff), _fbUser));
                var rv = converter(ff.Variation, out var typedValue) ? typedValue : defaultValue;
                return rv;
            }
            return defaultValue;
        }
        #endregion

        public void Track(FbUser user, string eventName) => Track(user, eventName, 1.0d);

        /// <inheritdoc/>
        public void Track(FbUser user, string eventName, double metricValue)
        {
        }

        public void Close()
        {
            Task.Run(CloseAsync).Wait();
        }
        public async Task CloseAsync()
        {
            _logger.LogInformation("Closing FbClient...");
            _dataSynchronizer.FeatureFlagsUpdated -= DataSynchronizer_FeatureFlagsUpdated;
            await _dataSynchronizer.StopAsync();
            _logger.LogInformation("FbClient successfully closed.");
        }

    }

    internal delegate bool ValueConverter<TValue>(string value, out TValue converted);

    internal static class ValueConverters
    {
        internal static readonly ValueConverter<bool> Bool = (string value, out bool converted) => bool.TryParse(value, out converted);
        internal static readonly ValueConverter<string> String = (string value, out string converted) =>
        {
            converted = value;
            return true;
        };
        public static readonly ValueConverter<int> Int = (string value, out int converted) => int.TryParse(value, out converted);
        public static readonly ValueConverter<float> Float = 
            (string value, out float converted) => float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out converted);
        public static readonly ValueConverter<double> Double = 
            (string value, out double converted) => double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out converted);
    }
}
