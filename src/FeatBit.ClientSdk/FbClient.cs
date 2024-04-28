using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatBit.ClientSdk.DataSynchronizer;
using FeatBit.ClientSdk.Evaluation;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;
using FeatBit.ClientSdk.Store;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public bool Initialized => _dataSynchronizer.Initialized;

        private readonly FbOptions _options;
        private readonly IEvaluator _evaluator;
        private readonly IMemoryStore _store;
        private IDataSynchronizer _dataSynchronizer;
        private FbUser _user;

        private readonly ILogger<FbClient> _logger;

        /// <summary>
        /// Creates a new FbClient instance, then starts fetching feature flags workflow.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In offline mode, this constructor will return immediately. Otherwise, it will wait and block on
        /// the current thread until initialization and the first response from the FeatBit is returned,
        /// up to the <see cref="FbOptions.StartWaitTime"/> timeout. If the timeout elapses, the returned instance will
        /// have an <see cref="Initialized"/> property of <see langword="false"/>.
        /// </para>
        /// </remarks>
        /// <param name="options">the client options</param>
        /// <param name="initialUser">the initial evaluation user; You specify this user at initialization time,
        /// and you can change it later with <see cref="IdentifyAsync(FbUser)"/>. All subsequent calls to evaluation
        /// methods like <see cref="BoolVariation(string, bool)"/> refer to the flag values for the current user.
        /// </param>
        public FbClient(FbOptions options, FbUser initialUser)
        {
            _options = options;
            _user = initialUser;

            _store = new DefaultMemoryStore();
            _evaluator = new Evaluator(_store);
            _dataSynchronizer = _options.DataSyncMode switch
            {
                DataSyncMode.Polling => _dataSynchronizer = new PollingDataSynchronizer(_options, _user, _store),
                _ => new NullDataSynchronizer()
            };

            _logger = _options.LoggerFactory.CreateLogger<FbClient>();

            // starts client
            Start();
        }

        private void Start()
        {
            _logger.LogInformation("Starting FbClient...");
            var task = _dataSynchronizer.StartAsync();
            try
            {
                var startWaitTime = _options.StartWaitTime.TotalMilliseconds;
                _logger.LogInformation(
                    "Waiting up to {StartWaitTime} milliseconds for FbClient to start...", startWaitTime
                );
                var success = task.Wait(_options.StartWaitTime);
                if (success)
                {
                    _logger.LogInformation("FbClient successfully started");
                }
                else
                {
                    _logger.LogError(
                        "FbClient failed to start successfully within {StartWaitTime} milliseconds. " +
                        "This error usually indicates a connection issue with FeatBit or an invalid secret. " +
                        "Please double-check your EnvSecret and StreamingUri configuration.",
                        startWaitTime
                    );
                }
            }
            catch (Exception ex)
            {
                // we do not want to throw exceptions from the FbClient constructor, so we'll just swallow this.
                _logger.LogError(ex, "An exception occurred during FbClient initialization.");
            }
        }

        public async Task IdentifyAsync(FbUser user)
        {
            _user = user;

            // dispose current data synchronizer and starts a new one
            _dataSynchronizer.Dispose();
            _dataSynchronizer = _options.DataSyncMode switch
            {
                DataSyncMode.Polling => _dataSynchronizer = new PollingDataSynchronizer(_options, _user, _store),
                _ => new NullDataSynchronizer()
            };

            // TODO: timeout
            await _dataSynchronizer.StartAsync();
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
            if (!Initialized)
            {
                // Flag evaluation before client initialized; always returning default value
                return new EvalDetail<TValue>("client not ready", defaultValue);
            }

            var evalResult = _evaluator.Evaluate(key);
            if (!evalResult.IsValid)
            {
                // error happened when evaluate flag, return default value 
                return new EvalDetail<TValue>(evalResult.Reason, defaultValue);
            }

            // TODO: send evaluation event

            return converter(evalResult.Value, out var typedValue)
                ? new EvalDetail<TValue>(evalResult.Reason, typedValue)
                // type mismatch, return default value
                : new EvalDetail<TValue>("type mismatch", defaultValue);
        }

        public void Dispose()
        {
            _dataSynchronizer?.Dispose();
        }
    }
}