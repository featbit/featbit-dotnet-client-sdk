using FeatBit.ClientSdk.Concurrent;
using FeatBit.ClientSdk.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    internal class PollingDataSynchronizer : IDataSynchronizer
    {
        public bool Initialized { get; private set; }

        private System.Timers.Timer _timer;
        private readonly FeatureFlagsCollection _store;
        private readonly FbOptions _options;
        private readonly TaskCompletionSource<bool> _initTcs;
        private readonly ILogger<PollingDataSynchronizer> _logger;
        private readonly IFeatBitRestfulService _apiService;

        private static readonly byte[] FullDataSync =
            Encoding.UTF8.GetBytes("{\"messageType\":\"data-sync\",\"data\":{\"timestamp\":0}}");

        public PollingDataSynchronizer(
            FbOptions options)
        {
            _store = FeatureFlagsCollection.Instance;

            // Shallow copy so we don't mutate the user-defined options object.
            var shallowCopiedOptions = options.ShallowCopy();
            _options = shallowCopiedOptions;

            //var factory = fbWebSocketFactory ?? DefaultFbWebSocketFactory;
            //_webSocket = factory(shallowCopiedOptions);

            //_webSocket.OnConnected += OnConnected;
            //_webSocket.OnReceived += OnReceived;

            _initTcs = new TaskCompletionSource<bool>();
            Initialized = false;

            _logger = options.LoggerFactory.CreateLogger<PollingDataSynchronizer>();
        }

        //private static FbWebSocket DefaultFbWebSocketFactory(FbOptions options)
        //{
        //    return new FbWebSocket(options);
        //}

        public Task<bool> StartAsync()
        {
            Task.Run(() =>
            {
                if (_options.PoollingInterval <= 0)
                {
                    throw new InvalidOperationException("Polling interval must be greater than 0");
                }
                _apiService.GetLatestAllAsync(FbIdentity.Instance).Wait();

                _timer = new System.Timers.Timer(_options.PoollingInterval);
                _timer.Elapsed += async (sender, e) => await DoDataSyncAsync();
                _timer.AutoReset = true;
                _timer.Start();
                //var cts = new CancellationTokenSource(_options.ConnectTimeout);
                //return _webSocket.ConnectAsync(cts.Token);
            });

            return _initTcs.Task;
        }

        private async Task OnConnected()
        {
            try
            {
                // do data-sync once the connection is established
                await DoDataSyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception occurred when performing data synchronization request");
            }
        }

        private Task OnReceived(ReadOnlySequence<byte> bytes)
        {
            try
            {
                HandleMessage(bytes);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Exception occurred when handling server message");
            }

            return Task.CompletedTask;
        }

        private async Task DoDataSyncAsync()
        {
            byte[] request;

            var version = _store.Version();
            if (version == 0)
            {
                // this should be the hot path
                request = FullDataSync;
            }
            else
            {
                object patchDataSync = new
                {
                    messageType = "data-sync",
                    data = new
                    {
                        timestamp = version
                    }
                };

                request = JsonSerializer.SerializeToUtf8Bytes(patchDataSync);
            }

            _logger.LogDebug("Do data-sync with version: {Version}", version);
            await _webSocket.SendAsync(request);
        }

        private void HandleMessage(ReadOnlySequence<byte> sequence)
        {
            var bytes = sequence.IsSingleSegment
                ? sequence.First // this should be the hot path
                : sequence.ToArray();

            using (var jsonDocument = JsonDocument.Parse(bytes))
            {
                var root = jsonDocument.RootElement;
                var messageType = root.GetProperty("messageType").GetString();

                // handle 'data-sync' message
                if (messageType == "data-sync")
                {
                    var dataSet = DataSet.FromJsonElement(root.GetProperty("data"));
                    _logger.LogDebug("Received {Type} data-sync message", dataSet.EventType);
                    var objects = dataSet.GetStorableObjects();
                    // populate data store
                    if (dataSet.EventType == DataSet.Full)
                    {
                        _store.Populate(objects);
                    }
                    // upsert objects
                    else if (dataSet.EventType == DataSet.Patch)
                    {
                        foreach (var storableObject in objects)
                        {
                            _store.Upsert(storableObject);
                        }
                    }

                    if (!Initialized)
                    {
                        CompleteInitialize();
                    }
                }
            }
        }

        private void CompleteInitialize()
        {
            Initialized = true;
            _initTcs.TrySetResult(true);
        }

        public async Task StopAsync()
        {
            _webSocket.OnConnected -= OnConnected;
            _webSocket.OnReceived -= OnReceived;

            await _webSocket.CloseAsync();
        }
    }
}
