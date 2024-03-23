using FeatBit.ClientSdk.Events;
using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            //_logger = options.LoggerFactory.CreateLogger<PollingDataSynchronizer>();

            // Shallow copy so we don't mutate the user-defined options object.
            var shallowCopiedOptions = options.ShallowCopy();
            _options = shallowCopiedOptions;

            _initTcs = new TaskCompletionSource<bool>();
            Initialized = false;

            GenerateDefaultUser();

            _timer = new System.Timers.Timer(_options.PoollingInterval);
        }

        public void GenerateDefaultUser()
        {
            _unloginUser = FbUser.Builder("unkown-" + Guid.NewGuid().ToString())
                .Name("unkown " + Guid.NewGuid().ToString())
                .Custom("ip", "unkown")
                .Custom("device", "windows")
                .Custom("application-type", "console")
                .Build();
            if (_fbUser == null)
                _fbUser = _unloginUser.ShallowCopy();
        }

        public void Identify(FbUser fbUser)
        {
            _fbUser = fbUser.ShallowCopy();
        }

        public async Task<bool> StartAsync()
        {
            RefreshFeatureFlagsCollection(await _apiService.GetLatestAllAsync(_fbUser));

            _timer.Elapsed += async (sender, e) => {
                RefreshFeatureFlagsCollection(await _apiService.GetLatestAllAsync(_fbUser));
            };
            _timer.AutoReset = true;
            _timer.Start();

            return true;
        }

        private void RefreshFeatureFlagsCollection(List<FeatureFlag> ffs)
        {
            List<FeatureFlag> changedItems = new List<FeatureFlag>();

            foreach (var item in ffs)
            {
                if (!_featureFlagsCollection.TryGetValue(item.Id, out var existingItem) || 
                    existingItem.Variation != item.Variation)
                {
                    changedItems.Add(item.ShallowCopy());
                }
                _featureFlagsCollection.AddOrUpdate(item.Id, item.ShallowCopy(), (existingKey, existingValue) => item.ShallowCopy());
            }

            var args = new FeatureFlagsUpdatedEventArgs();
            args.UpdatedFeatureFlags = changedItems;
            OnFeatureFlagsUpdated(args);
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
            _timer.Elapsed -= async (sender, e) => await DoDataSyncAsync();
            _timer.Stop();
            _timer.Close();
        }
    }
}
