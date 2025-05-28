using System;
using System.Threading;
using System.Threading.Tasks;
using FeatBit.Sdk.Client.Concurrent;
using FeatBit.Sdk.Client.Internal;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using FeatBit.Sdk.Client.Store;
using FeatBit.Sdk.Client.Utils;
using Microsoft.Extensions.Logging;

namespace FeatBit.Sdk.Client.DataSynchronizer
{
    internal sealed class PollingDataSynchronizer : IDataSynchronizer
    {
        private readonly TaskCompletionSource<bool> _startTask;
        private readonly AtomicBoolean _initialized = new AtomicBoolean(false);

        private readonly TimeSpan _pollingInterval;
        private readonly IGetUserFlags _getUserFlags;
        private readonly string _userKey;
        private readonly IMemoryStore _store;
        private readonly ILogger<PollingDataSynchronizer> _logger;

        private long _timestamp;
        private CancellationTokenSource _canceller;

        public bool Initialized => _initialized;

        public PollingDataSynchronizer(FbOptions options, FbUser user, IMemoryStore store)
        {
            _startTask = new TaskCompletionSource<bool>();
            _pollingInterval = options.PollingInterval;

            _userKey = user.Key;
            _store = store;
            _timestamp = 0;
            _getUserFlags = new GetUserFlags(options, user);

            _logger = options.LoggerFactory.CreateLogger<PollingDataSynchronizer>();
        }

        public Task<bool> StartAsync()
        {
            StartPollingAsync().Forget();

            return _startTask.Task;
        }

        private async Task StartPollingAsync()
        {
            _canceller = new CancellationTokenSource();
            while (!_canceller.IsCancellationRequested)
            {
                await SafePollAsync().ConfigureAwait(false);

                try
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug(
                            "Waiting for the next polling interval of {PollingInterval}s.",
                            _pollingInterval.TotalSeconds
                        );
                    }

                    await Task.Delay(_pollingInterval, _canceller.Token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while waiting for the next polling interval.");
                }
            }

            // dispose the cancellable token source when polling is stopped
            _canceller.Dispose();
            _canceller = null;
        }

        private async Task SafePollAsync()
        {
            try
            {
                var response = await _getUserFlags.RunAsync(_timestamp).ConfigureAwait(false);
                if (response.IsFatal)
                {
                    _logger.LogError(
                        "Polling data synchronizer encountered fatal HTTP error {StatusCode}. Stop polling...",
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

                _timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        "Polling data at {Time:u} received {Count} new flags.",
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
                    _logger.LogInformation("Polling data synchronizer initialized for user {UserId}.", _userKey);
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
            _getUserFlags?.Dispose();
        }
    }
}