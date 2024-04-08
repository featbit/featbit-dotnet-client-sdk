using FeatBit.ClientSdk.Events;
using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    internal class PollingDataSynchronizer : IDataSynchronizer
    {
        public event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;
        private readonly SemaphoreSlim _syncSemaphore = new SemaphoreSlim(1, 1);

        private const int _timerInitTimeSpan = 1;
        private readonly System.Timers.Timer _timer;
        private readonly FbOptions _options;
        private readonly ILogger<PollingDataSynchronizer> _logger;
        private readonly IFeatBitRestfulService _apiService;
        private readonly ConcurrentDictionary<String, FeatureFlag> _featureFlagsCollection;
        private FbUser _fbUser;

        public PollingDataSynchronizer(
            FbOptions options,
            ConcurrentDictionary<String, FeatureFlag> featureFlagsCollection)
        {
            _featureFlagsCollection = featureFlagsCollection;
            _apiService = new FeatBitRestfulService(options);
            _logger = options.LoggerFactory.CreateLogger<PollingDataSynchronizer>();
            _options = options.ShallowCopy();
            _timer = new System.Timers.Timer();
        }

        public void Identify(FbUser fbUser)
        {
            _fbUser = fbUser.ShallowCopy();
        }

        public async Task StartAsync()
        {
            _timer.Interval = _timerInitTimeSpan;
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = false;
            _timer.Start();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_timer.Interval == _timerInitTimeSpan)
            {
                _timer.Stop();
                _timer.Interval = _options.PoollingInterval;
                _timer.AutoReset = true;
                _timer.Start();
            }
            Task.Run(async () =>
            {
                var newFfs = await _apiService.GetLatestAllAsync(_fbUser.ShallowCopy());
                UpdateFeatureFlagsCollection(newFfs);
            });
        }

        private async Task TimerElapsedAsync(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(_timer.Interval == _timerInitTimeSpan)
            {
                _timer.Stop();
                _timer.Interval = _options.PoollingInterval;
                _timer.AutoReset = true;
                _timer.Start();
            }
            var newFfs = await _apiService.GetLatestAllAsync(_fbUser);
            UpdateFeatureFlagsCollection(newFfs);
        }

        public async Task UpdateFeatureFlagCollectionAsync()
        {
            await _syncSemaphore.WaitAsync();
            try
            {
                var newFfs = await _apiService.GetLatestAllAsync(_fbUser);
                UpdateFeatureFlagsCollection(newFfs);
            }
            finally
            {
                _syncSemaphore.Release();
            }
        }

        public void UpdateFeatureFlagsCollection(List<FeatureFlag> ffs)
        {
            try
            {
                List<FeatureFlag> changedItems = new List<FeatureFlag>();

                _logger.LogInformation($"Latest Feature Flags Retrieving Started @ {DateTime.Now.ToString()}");

                foreach (var item in ffs)
                {
                    if (!_featureFlagsCollection.TryGetValue(item.Id, out var existingItem) ||
                        existingItem.Variation != item.Variation)
                    {
                        changedItems.Add(item.ShallowCopy());
                        _featureFlagsCollection.AddOrUpdate(item.Id, item.ShallowCopy(), (existingKey, existingValue) => item.ShallowCopy());
                    }
                }

                _logger.LogInformation($"Latest Feature Flags Retrieving Completed @ {DateTime.Now.ToString()}");

                var args = new FeatureFlagsUpdatedEventArgs();
                args.UpdatedFeatureFlags = changedItems;
                OnFeatureFlagsUpdated(args);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Latest Feature Flags Retrieving Error @ {DateTime.Now.ToString()}");
            }
        }

        protected virtual void OnFeatureFlagsUpdated(FeatureFlagsUpdatedEventArgs e)
        {
            EventHandler<FeatureFlagsUpdatedEventArgs> handler = FeatureFlagsUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public async Task StopAsync()
        {
            FeatureFlagsUpdated = null;
            _timer.Elapsed -= TimerElapsed;
            _timer.Stop();
        }

        public async Task CloseTimerAsync()
        {
            await StopAsync();
            _timer.Close();
        }
    }
}
