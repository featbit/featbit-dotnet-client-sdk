using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Options;
using Microsoft.Extensions.Logging;

namespace FeatBit.ClientSdk.Internal
{
    internal abstract class FbApiClient : IDisposable
    {
        private static readonly TimeSpan DefaultConnectTimeout = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan DefaultReadTimeout = TimeSpan.FromSeconds(8);
        private static readonly TimeSpan DefaultTimeout = DefaultConnectTimeout + DefaultReadTimeout;

        private static readonly MediaTypeHeaderValue JsonContentType = new MediaTypeHeaderValue("application/json")
        {
            CharSet = "utf-8"
        };

        private readonly FbOptions _options;
        private readonly HttpClient _httpClient;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly ILogger<FbApiClient> _logger;

        protected static readonly JsonSerializerOptions DefaultSerializerOptions
            = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        protected abstract Func<FbOptions, Uri> BaseAddressGetter { get; }

        protected FbApiClient(
            FbOptions options,
            Func<FbOptions, HttpClient> customHttpClientFactory = null)
        {
            _options = options;

            var httpClientFactory = customHttpClientFactory ?? DefaultHttpClientFactory;
            _httpClient = httpClientFactory(options);

            _logger = options.LoggerFactory.CreateLogger<FbApiClient>();
        }

        protected async Task<HttpResponseMessage> PostAsync(string path, byte[] payload)
        {
            // check log level to avoid unnecessary string allocation
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var fullUri = new Uri(BaseAddressGetter(_options), path).ToString();
                var stringPayload = Encoding.UTF8.GetString(payload);
                _logger.LogDebug("HTTP POST {Uri} with {Payload}", fullUri, stringPayload);
            }

            using var content = new ByteArrayContent(payload);
            content.Headers.ContentType = JsonContentType;

            _stopwatch.Restart();
            using var cts = new CancellationTokenSource(DefaultTimeout);
            var response = await _httpClient.PostAsync(path, content, cts.Token);
            _stopwatch.Stop();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Api call took {ElapsedMs} ms, response status {Status}. The body is {Body}",
                    _stopwatch.ElapsedMilliseconds,
                    (int)response.StatusCode,
                    response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty
                );
            }

            return response;
        }

        private HttpClient DefaultHttpClientFactory(FbOptions options)
        {
#if NETCOREAPP || NET6_0
            var handler = new SocketsHttpHandler
            {
                ConnectTimeout = DefaultConnectTimeout
            };
#else
            var handler = new HttpClientHandler();
#endif
            var client = new HttpClient(handler, false);
            client.BaseAddress = BaseAddressGetter(options);
            client.DefaultRequestHeaders.Add("Authorization", options.Secret);
            client.DefaultRequestHeaders.Add("User-Agent", "fb-dotnet-client-sdk");
            return client;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}