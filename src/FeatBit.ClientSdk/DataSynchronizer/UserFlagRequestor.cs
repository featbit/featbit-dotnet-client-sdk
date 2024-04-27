using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FeatBit.ClientSdk.Model;
using FeatBit.ClientSdk.Options;

namespace FeatBit.ClientSdk.DataSynchronizer
{
    internal struct GetFeatureFlagsResponse
    {
        public int StatusCode { get; private set; }

        public FeatureFlag[] Flags { get; private set; }

        public static GetFeatureFlagsResponse Ok(FeatureFlag[] flags) => new GetFeatureFlagsResponse
        {
            StatusCode = 200,
            Flags = flags
        };

        public static GetFeatureFlagsResponse Error(int statusCode) => new GetFeatureFlagsResponse
        {
            StatusCode = statusCode,
            Flags = Array.Empty<FeatureFlag>()
        };

        public bool IsError => StatusCode != 200;

        public bool IsFatal => StatusCode == 401;
    }

    internal interface IUserFlagRequestor : IDisposable
    {
        Task<GetFeatureFlagsResponse> GetFeatureFlagsAsync();
    }

    internal sealed class UserFlagRequestor : IUserFlagRequestor
    {
        private static readonly TimeSpan DefaultConnectTimeout = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan DefaultReadTimeout = TimeSpan.FromSeconds(8);
        private static readonly TimeSpan DefaultTimeout = DefaultConnectTimeout + DefaultReadTimeout;

        private static readonly JsonSerializerOptions DefaultSerializerOptions
            = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        private const string PollingPath = "api/public/sdk/client/latest-all";

        private readonly string _payload;
        private readonly HttpClient _httpClient;
        private long _timestamp;

        public UserFlagRequestor(
            FbOptions options,
            FbUser user,
            Func<FbOptions, HttpClient> customHttpClientFactory = null)
        {
            var jsonObject = new
            {
                keyId = user.Key,
                name = user.Name,
                customizedProperties = new List<KeyValuePair<string, string>>(user.Custom)
            };
            _payload = JsonSerializer.Serialize(jsonObject, DefaultSerializerOptions);

            var httpClientFactory = customHttpClientFactory ?? DefaultHttpClientFactory;
            _httpClient = httpClientFactory(options);

            _timestamp = 0;
        }

        public async Task<GetFeatureFlagsResponse> GetFeatureFlagsAsync()
        {
            using var cts = new CancellationTokenSource(DefaultTimeout);

            var body = new StringContent(_payload, Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(
                $"?timestamp={_timestamp}",
                body,
                cts.Token
            );

            var statusCode = (int)response.StatusCode;
            if (!response.IsSuccessStatusCode)
            {
                return GetFeatureFlagsResponse.Error(statusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return GetFeatureFlagsResponse.Ok(Array.Empty<FeatureFlag>());
            }

            using var json = JsonDocument.Parse(content);
            var flags = json.RootElement
                .GetProperty("data")
                .GetProperty("featureFlags")
                .Deserialize<FeatureFlag[]>(DefaultSerializerOptions);

            _timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return GetFeatureFlagsResponse.Ok(flags);
        }

        private static HttpClient DefaultHttpClientFactory(FbOptions options)
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
            client.BaseAddress = new Uri(options.PollingUri, PollingPath);
            client.DefaultRequestHeaders.Add("Authorization", options.Secret);
            return client;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}