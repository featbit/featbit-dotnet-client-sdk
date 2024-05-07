using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FeatBit.Sdk.Client.Model;
using FeatBit.Sdk.Client.Options;
using Microsoft.Extensions.Logging;

namespace FeatBit.Sdk.Client.Internal
{
    internal interface ITrackInsight : IDisposable
    {
        Task RunAsync(Insight insight);
    }

    internal class NoopTrackInsight : ITrackInsight
    {
        public void Dispose()
        {
        }

        public Task RunAsync(Insight insight)
        {
            return Task.CompletedTask;
        }
    }

    internal class TrackInsight : FbApiClient, ITrackInsight
    {
        private readonly ILogger<TrackInsight> _trackInsightLogger;

        public TrackInsight(FbOptions options, Func<FbOptions, HttpClient> customHttpClientFactory = null)
            : base(options, customHttpClientFactory)
        {
            _trackInsightLogger = options.LoggerFactory.CreateLogger<TrackInsight>();
        }

        protected override Func<FbOptions, Uri> BaseAddressGetter => options => new Uri(
            options.EventUri,
            "api/public/insight/track"
        );

        public async Task RunAsync(Insight insight)
        {
            try
            {
                var insights = new Insight[1];
                insights[0] = insight;

                var payload = JsonSerializer.SerializeToUtf8Bytes(insights, DefaultSerializerOptions);
                await PostAsync(string.Empty, payload).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _trackInsightLogger.LogError(ex, "Exception occurred while tracking insight.");
            }
        }
    }
}