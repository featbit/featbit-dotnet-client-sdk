using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatBit.Sdk.Client.ChangeTracker;
using FeatBit.Sdk.Client.Concurrent;
using FeatBit.Sdk.Client.DataSynchronizer;
using FeatBit.Sdk.Client.Evaluation;
using FeatBit.Sdk.Client.Internal;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using FeatBit.Sdk.Client.Store;
using Microsoft.Extensions.Logging;

namespace FeatBit.Sdk.Client
{
    public class FbClient : IFbClient
    {
        public bool Initialized => _dataSynchronizer.Initialized;

        private readonly FbOptions _options;
        private readonly IEvaluator _evaluator;
        private readonly IMemoryStore _store;
        private readonly ITrackInsight _trackInsight;
        private readonly IFlagTracker _flagTracker;
        private IDataSynchronizer _dataSynchronizer;
        private FbUser _user;

        private readonly ILogger<FbClient> _logger;

        /// <inheritdoc/>
        public IFlagTracker FlagTracker => _flagTracker;

        /// <summary>
        /// Creates a new FbClient instance
        /// </summary>
        /// <param name="options">the client options</param>
        /// <param name="initialUser">the initial evaluation user; You specify this user at initialization time,
        /// and you can change it later with <see cref="IdentifyAsync(FbUser, TimeSpan?)"/>. All subsequent calls to evaluation
        /// methods like <see cref="BoolVariation(string, bool)"/> refer to the flag values for the current user.
        /// </param>
        public FbClient(FbOptions options, FbUser initialUser)
        {
            _options = options;
            _user = initialUser;

            _store = new DefaultMemoryStore(_options.Bootstrap);
            _evaluator = new Evaluator(_store);
            _flagTracker = new FlagTracker(_store);

            if (_options.Offline)
            {
                _trackInsight = new NoopTrackInsight();
                _dataSynchronizer = new NullDataSynchronizer();
            }
            else
            {
                _trackInsight = new TrackInsight(options);
                _dataSynchronizer = _options.DataSyncMode switch
                {
                    DataSyncMode.Polling => new PollingDataSynchronizer(_options, _user, _store),
                    _ => new NullDataSynchronizer()
                };
            }

            _logger = _options.LoggerFactory.CreateLogger<FbClient>();
        }

        /// <inheritdoc/>
        public async Task<bool> StartAsync(TimeSpan? startTimeout = null)
        {
            var timeout = startTimeout ?? TimeSpan.FromSeconds(3);

            _logger.LogInformation(
                "Waiting up to {StartWaitTime} milliseconds for FbClient to start...", timeout.TotalMilliseconds
            );

            try
            {
                var success = await _dataSynchronizer.StartAsync().WithTimeout(timeout)
                    .ConfigureAwait(false);
                if (success)
                {
                    _logger.LogInformation("FbClient successfully started");
                }

                return success;
            }
            catch (OperationCanceledException)
            {
                _logger.LogError(
                    "FbClient failed to start successfully within {StartWaitTime} milliseconds. " +
                    "This error usually indicates a connection issue with FeatBit or an invalid secret. " +
                    "Please double-check your EnvSecret and StreamingUri configuration.",
                    timeout
                );

                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IdentifyAsync(FbUser user, TimeSpan? identifyTimeout = null)
        {
            _user = user;

            // dispose current data synchronizer and starts a new one
            _dataSynchronizer.Dispose();
            _dataSynchronizer = _options.DataSyncMode switch
            {
                DataSyncMode.Polling => new PollingDataSynchronizer(_options, _user, _store),
                _ => new NullDataSynchronizer()
            };

            try
            {
                var timeout = identifyTimeout ?? TimeSpan.FromSeconds(3);
                var success = await _dataSynchronizer.StartAsync().WithTimeout(timeout).ConfigureAwait(false);
                return success;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool BoolVariation(string key, bool defaultValue = false)
            => EvaluateCore(key, defaultValue, ValueConverters.Bool).Value;

        /// <inheritdoc/>
        public EvalDetail<bool> BoolVariationDetail(string key, bool defaultValue = false)
            => EvaluateCore(key, defaultValue, ValueConverters.Bool);

        /// <inheritdoc/>
        public int IntVariation(string key, int defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.Int).Value;

        /// <inheritdoc/>
        public EvalDetail<int> IntVariationDetail(string key, int defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.Int);

        /// <inheritdoc/>
        public float FloatVariation(string key, float defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.Float).Value;

        /// <inheritdoc/>
        public EvalDetail<float> FloatVariationDetail(string key, float defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.Float);

        /// <inheritdoc/>
        public double DoubleVariation(string key, double defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.Double).Value;

        /// <inheritdoc/>
        public EvalDetail<double> DoubleVariationDetail(string key, double defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.Double);

        /// <inheritdoc/>
        public string StringVariation(string key, string defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.String).Value;

        /// <inheritdoc/>
        public EvalDetail<string> StringVariationDetail(string key, string defaultValue)
            => EvaluateCore(key, defaultValue, ValueConverters.String);

        public IDictionary<string, FeatureFlag> AllFlags()
        {
            var flags = _store.GetAll();

            return flags.ToDictionary(x => x.Id, x => x);
        }

        private EvalDetail<TValue> EvaluateCore<TValue>(
            string key,
            TValue defaultValue,
            ValueConverter<TValue> converter)
        {
            // if the client is not ready and no bootstrap flags
            if (!Initialized && _options.Bootstrap.Length == 0)
            {
                // Flag evaluation before client initialized; always returning default value
                return new EvalDetail<TValue>("client not ready", defaultValue);
            }

            var (evalResult, featureFlag) = _evaluator.Evaluate(key);
            if (!evalResult.IsValid)
            {
                // error happened when evaluate flag, return default value 
                return new EvalDetail<TValue>(evalResult.Reason, defaultValue);
            }

            var insight = new Insight(_user, featureFlag);
            _ = Task.Run(() => _trackInsight.RunAsync(insight));

            return converter(evalResult.Value, out var typedValue)
                ? new EvalDetail<TValue>(evalResult.Reason, typedValue)
                // type mismatch, return default value
                : new EvalDetail<TValue>("type mismatch", defaultValue);
        }

        public void Dispose()
        {
            _dataSynchronizer?.Dispose();
            _flagTracker?.Dispose();
        }
    }
}