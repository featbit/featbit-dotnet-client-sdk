using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;

namespace FeatBit.Sdk.Client.Internal
{
    internal interface ITrackInsight : IDisposable
    {
        Task RunAsync(Insight insight);
    }

    internal class TrackInsight : FbApiClient, ITrackInsight
    {
        public TrackInsight(FbOptions options, Func<FbOptions, HttpClient> customHttpClientFactory = null)
            : base(options, customHttpClientFactory)
        {
        }

        protected override Func<FbOptions, Uri> BaseAddressGetter => options => new Uri(
            options.EventUri,
            "api/public/insight/track"
        );

        public async Task RunAsync(Insight insight)
        {
            var insights = new Insight[1];
            insights[0] = insight;

            var payload = JsonSerializer.SerializeToUtf8Bytes(insights, DefaultSerializerOptions);
            await PostAsync(string.Empty, payload);
        }
    }
}