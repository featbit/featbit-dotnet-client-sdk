using FeatBit.ClientSdk.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk.Services
{
    public interface IInsightsAndEventSenderService
    {
        Task TrackInsightAsync(VariationInsight evt, FbUser fbUser, CancellationTokenSource cts = null);
        Task TrackMetricAsync(MetricInsight metricEvent, CancellationTokenSource cts = null);
    }

    public class InsightsAndEventSenderService : IInsightsAndEventSenderService
    {
        private readonly FbOptions _options;
        private readonly ILogger<InsightsAndEventSenderService> _logger;
        private readonly HttpClient _httpClient;
        private readonly HttpsClientHandlerService _httpsClientHandlerService;

        public InsightsAndEventSenderService(FbOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<InsightsAndEventSenderService>();

#if DEBUG && (ANDROID || IOS)
            string eventUri = _options.EvalUri.ToString();
            _httpsClientHandlerService = new HttpsClientHandlerService(eventUri);
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
            if (handler != null)
                _httpClient = new HttpClient(handler);
            else
                _httpClient = new HttpClient();
#else
            _httpClient = new HttpClient();
#endif
        }

        public async Task TrackInsightAsync(VariationInsight evt, FbUser fbUser, CancellationTokenSource cts = null)
        {
            var url = $"{_options.EvalUri}api/public/insight/track";
            var requestBody = new List<Insight>()
            {
                new Insight()
                {
                    User = new EndUser
                    {
                        KeyId = fbUser.Key,
                        Name = fbUser.Name,
                        CustomizedProperties = fbUser.Custom?.Select(c => new CustomizedProperty { Name = c.Key, Value = c.Value }).ToArray() ?? Array.Empty<CustomizedProperty>()
                    },
                    Metrics = new List<MetricInsight>(),
                    Variations = new List<VariationInsight>
                    {
                        evt.ShallowCopy()
                    }
                }
            };


            _httpClient.DefaultRequestHeaders.Add("Authorization", _options.EnvSecret);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var contentStr = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(contentStr, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content, cts == null ? CancellationToken.None : cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("New feature flag insight have been sent successfully");
                }
                else
                {
                    _logger.LogError($"Failed to tracked feature flag insight; StatusCode: {response.StatusCode};");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to tracked feature flag insight");
                throw ex;
            }
        }


        public async Task TrackMetricAsync(MetricInsight metricEvent, CancellationTokenSource cts = null)
        {

        }
    }
}
