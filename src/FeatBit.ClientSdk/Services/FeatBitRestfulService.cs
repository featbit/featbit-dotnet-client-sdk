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
    public interface IFeatBitRestfulService
    {
        Task<List<FeatureFlag>> GetLatestAllAsync(FbUser identity, CancellationTokenSource cts = null);
    }

    public class FeatBitRestfulService: IFeatBitRestfulService
    {
        private readonly FbOptions _options;
        private readonly ILogger<FeatBitRestfulService> _logger;
        public FeatBitRestfulService(FbOptions options)
        {
            _options = options;
            _logger = options.LoggerFactory.CreateLogger<FeatBitRestfulService>();
        }

        public async Task<List<FeatureFlag>> GetLatestAllAsync(FbUser identity, CancellationTokenSource cts = null)
        {
            var url = $"{_options.EventUri}api/public/sdk/client/latest-all";
            var requestBody = new
            {
                keyId = identity.Key,
                name = identity.Name,
                customizedProperties = identity.Custom.ToArray()
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", _options.EnvSecret);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var contentStr = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(contentStr, Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                try
                {
                    var response = await httpClient.PostAsync(url, content, cts == null ? CancellationToken.None : cts.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Response received successfully:");
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<List<FeatureFlag>>(responseContent) ?? new List<FeatureFlag>();
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve feature flags from  api client/latest-all");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to retrieve feature flags from api client/latest-all");
                    throw ex;
                }
            }
        }
    }
}
