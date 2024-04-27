using System;
using System.Threading;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Concurrent;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;
using FeatBit.ClientSdk.Store;
using Microsoft.Extensions.Logging;

namespace FeatBit.ClientSdk.DataSynchronizer
{
    internal sealed class PollingDataSynchronizer : IDataSynchronizer
    {
        private readonly TaskCompletionSource<bool> _startTask;
        private readonly AtomicBoolean _initialized = new AtomicBoolean(false);

        private readonly TimeSpan _pollingInterval;
        private readonly IUserFlagRequestor _requestor;
        private readonly IMemoryStore _store;

        private CancellationTokenSource _canceller;

        public bool Initialized => _initialized;

        private readonly ILogger<PollingDataSynchronizer> _logger;

        public PollingDataSynchronizer(FbOptions options, FbUser user, IMemoryStore store)
        {
            _startTask = new TaskCompletionSource<bool>();
            _pollingInterval = options.PollingInterval;

            _store = store;
            _requestor = new UserFlagRequestor(options, user);

            _logger = options.LoggerFactory.CreateLogger<PollingDataSynchronizer>();
        }

        public Task<bool> StartAsync()
        {
            _ = StartPollingAsync();

            return _startTask.Task;
        }

        private async Task StartPollingAsync()
        {
            _canceller = new CancellationTokenSource();
            while (!_canceller.IsCancellationRequested)
            {
                var nextTime = DateTime.Now.Add(_pollingInterval);

                await SafePollAsync();

                var timeToWait = nextTime.Subtract(DateTime.Now);
                if (timeToWait.CompareTo(TimeSpan.Zero) > 0)
                {
                    try
                    {
                        await Task.Delay(timeToWait, _canceller.Token);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }
            }
        }

        private async Task SafePollAsync()
        {
            try
            {
                var response = await _requestor.GetFeatureFlagsAsync();
                if (response.IsFatal)
                {
                    _logger.LogError(
                        "Polling data synchronizer encountered fatal HTTP error {StatusCode}. Stopping...",
                        response.StatusCode
                    );

                    _startTask.TrySetResult(false);
                    Dispose();
                    return;
                }

                if (response.IsError)
                {
                    _logger.LogWarning(
                        "Polling data synchronizer encountered transient HTTP error {StatusCode}.",
                        response.StatusCode
                    );

                    return;
                }

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        "Polling data at {Time:u} received {Count} flags.",
                        DateTime.UtcNow,
                        response.Flags.Length
                    );
                }

                var flags = response.Flags;
                for (var i = 0; i < flags.Length; i++)
                {
                    _store.Upsert(flags[i]);
                }

                if (_initialized.CompareAndSet(false, true))
                {
                    _startTask.SetResult(true);
                    _logger.LogInformation("Initialized polling data synchronizer.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while polling data.");
            }
        }

        public void Dispose()
        {
            _canceller?.Cancel();
            _canceller = null;

            _requestor?.Dispose();
        }
    }
}