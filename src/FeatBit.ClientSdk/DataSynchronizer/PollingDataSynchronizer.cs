using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    internal class PollingDataSynchronizer : IDataSynchronizer
    {
        public bool Initialized { get; private set; }

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

        public Task<bool> StartAsync()
        {
            Task.Run(() =>
            {
                var cts = new CancellationTokenSource(_options.ConnectTimeout);
                return FirstTimeCallApi(cts);
            });
            
            _timer.Elapsed += async (sender, e) => await DoDataSyncAsync();
            _timer.AutoReset = true;
            _timer.Start();

            return _initTcs.Task;
        }

        private async Task FirstTimeCallApi(CancellationTokenSource cts)
        { 
            var ffs = await _apiService.GetLatestAllAsync(_fbUser, cts);
            if (ffs != null && ffs.Count > 0)
            {
                foreach (var item in ffs)
                {
                    _featureFlagsCollection.AddOrUpdate(item.Id, item, (existingKey, existingValue) => item);
                }
                if (_initTcs.Task.IsCompleted == false)
                    CompleteInitialize();
            }
            else
            {
                var ex = new TimeoutException("Data synchronization timed out.");
                _logger.LogError(ex, ex.Message);
                if (_initTcs.Task.IsCompleted == false)
                    _initTcs.TrySetException(ex);
            }
        }

        private async Task DoDataSyncAsync()
        {
            var cts = new CancellationTokenSource(_options.ConnectTimeout);
            var ffsTask = _apiService.GetLatestAllAsync(_fbUser, cts);
            var delayTask = Task.Delay(_options.ConnectTimeout);
            var completedTask = await Task.WhenAny(ffsTask, delayTask);
            if (completedTask == ffsTask)
            {
                var ffs = await ffsTask;
                if (ffs != null && ffs.Count > 0)
                {
                    foreach (var item in ffs)
                    {
                        _featureFlagsCollection.AddOrUpdate(item.Id, item, (existingKey, existingValue) => item);
                    }
                }
            }
            else
            {
                cts.Cancel();
                var ex = new TimeoutException("Data synchronization timed out.");
                _logger.LogError(ex, ex.Message);
            }
        }

        private void CompleteInitialize()
        {
            Initialized = true;
            _initTcs.TrySetResult(true);
        }

        public async Task StopAsync()
        {
            _timer.Elapsed -= async (sender, e) => await DoDataSyncAsync();
            _timer.Stop();
            _timer.Close();
        }
    }
}
