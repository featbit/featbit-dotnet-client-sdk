using FeatBit.ClientSdk.Events;
using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    internal class PollingDataSynchronizer : IDataSynchronizer
    {
        public bool Initialized { get; private set; }
        public event EventHandler<FeatureFlagsUpdatedEventArgs> FeatureFlagsUpdated;

        private readonly System.Timers.Timer _timer;
        private readonly FbOptions _options;
        private readonly TaskCompletionSource<bool> _initTcs;
        private readonly ILogger<PollingDataSynchronizer> _logger;
        private readonly IFeatBitRestfulService _apiService;
        private readonly ConcurrentDictionary<String, FeatureFlag> _featureFlagsCollection;
        private FbUser _fbUser;
        private FbUser _unloginUser;


        public PollingDataSynchronizer(
            FbOptions options,
            ConcurrentDictionary<String, FeatureFlag> featureFlagsCollection)
        {
            _featureFlagsCollection = featureFlagsCollection;
            _apiService = new FeatBitRestfulService(options);
            _logger = options.LoggerFactory.CreateLogger<PollingDataSynchronizer>();

            // Shallow copy so we don't mutate the user-defined options object.
            var shallowCopiedOptions = options.ShallowCopy();
            _options = shallowCopiedOptions;

            _initTcs = new TaskCompletionSource<bool>();
            Initialized = false;

            _timer = new System.Timers.Timer(1);
        }

        public void Identify(FbUser fbUser)
        {
            _fbUser = fbUser.ShallowCopy();
        }

        public async Task StartAsync()
        {
            _timer.Elapsed += async (sender, e) => await TimerElapsedAsync(sender, e);
            _timer.AutoReset = false;
            _timer.Start();
        }

        private async Task TimerElapsedAsync(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(_timer.Interval == 1)
            {
                _timer.Stop();
                _timer.Interval = _options.PoollingInterval;
                _timer.AutoReset = true;
                _timer.Start();
            }
            var newFfs = await _apiService.GetLatestAllAsync(_fbUser);
            RefreshFeatureFlagsCollection(newFfs);
        }

        private void RefreshFeatureFlagsCollection(List<FeatureFlag> ffs)
        {
            try
            {
                List<FeatureFlag> changedItems = new List<FeatureFlag>();

                _logger.LogInformation($"Latest Feature Flags Retrieving Started @ {DateTime.Now.ToString()}");

                foreach (var item in ffs)
                {
                    //if (!_featureFlagsCollection.TryGetValue(item.Id, out var existingItem) ||
                    //    existingItem.Variation != item.Variation)
                    //{
                    //    changedItems.Add(item.ShallowCopy());
                    //}
                    changedItems.Add(item.ShallowCopy());
                    _featureFlagsCollection.AddOrUpdate(item.Id, item.ShallowCopy(), (existingKey, existingValue) => item.ShallowCopy());
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
            _timer.Elapsed -= async (sender, e) => await TimerElapsedAsync(sender, e);
            _timer.Stop();
            _timer.Close();
        }
    }
}
