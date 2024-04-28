using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;

namespace FeatBit.Sdk.Client.Internal
{
    internal struct GetUserFlagsResponse
    {
        public int StatusCode { get; private set; }

        public FeatureFlag[] Flags { get; private set; }

        public static GetUserFlagsResponse Ok(FeatureFlag[] flags) => new GetUserFlagsResponse
        {
            StatusCode = 200,
            Flags = flags
        };

        public static GetUserFlagsResponse Error(int statusCode) => new GetUserFlagsResponse
        {
            StatusCode = statusCode,
            Flags = Array.Empty<FeatureFlag>()
        };

        public bool IsError => StatusCode != 200;

        public bool IsFatal => StatusCode == 401;
    }

    internal interface IGetUserFlags : IDisposable
    {
        Task<GetUserFlagsResponse> RunAsync(long timestamp);
    }

    internal class GetUserFlags : FbApiClient, IGetUserFlags
    {
        private readonly byte[] _payload;

        protected override Func<FbOptions, Uri> BaseAddressGetter => options => new Uri(
            options.PollingUri,
            "api/public/sdk/client/latest-all"
        );

        public GetUserFlags(
            FbOptions options,
            FbUser user,
            Func<FbOptions, HttpClient> customHttpClientFactory = null) : base(options, customHttpClientFactory)
        {
            _payload = JsonSerializer.SerializeToUtf8Bytes(user.AsEndUser(), DefaultSerializerOptions);
        }

        public async Task<GetUserFlagsResponse> RunAsync(long timestamp)
        {
            using var response = await PostAsync($"?timestamp={timestamp}", _payload);

            var statusCode = (int)response.StatusCode;
            if (!response.IsSuccessStatusCode)
            {
                return GetUserFlagsResponse.Error(statusCode);
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return GetUserFlagsResponse.Ok(Array.Empty<FeatureFlag>());
            }

            using var json = JsonDocument.Parse(content);
            var flags = json.RootElement
                .GetProperty("data")
                .GetProperty("featureFlags")
                .Deserialize<FeatureFlag[]>(DefaultSerializerOptions);

            return GetUserFlagsResponse.Ok(flags);
        }
    }
}