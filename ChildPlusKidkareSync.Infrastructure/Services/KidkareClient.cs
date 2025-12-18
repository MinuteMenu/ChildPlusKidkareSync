using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace ChildPlusKidkareSync.Infrastructure.Services
{
    // ==================== HTTP CLIENT WRAPPER ====================
    public class KidkareClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<KidkareClient> _logger;

        public KidkareClient(string baseUrl, string apiKey, ILogger<KidkareClient> logger)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("mm-api-key", apiKey);
            _logger = logger;
        }

        // Send request + handle error
        private async Task<string> SendAsync(
            HttpMethod method,
            string endpoint,
            object payload = null,
            CancellationToken cancellationToken = default)
        {
            var url = new Uri(new Uri(_baseUrl), endpoint);
            using var request = new HttpRequestMessage(method, url);

            if (payload != null)
            {
                var json = JsonConvert.SerializeObject(payload, JsonHelper.DefaultJsonSettings);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogDebug("Sending {Method} request to {Endpoint} with payload: {Payload}", method, endpoint, json);
            }

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("{Method} {Endpoint} failed with {StatusCode} {Reason}. Response: {Response}",
                    method, endpoint, (int)response.StatusCode, response.ReasonPhrase, responseBody);

                throw new HttpRequestException($"{method} {endpoint} failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response body: {responseBody}");
            }

            _logger.LogDebug("Received response from {Endpoint}: {Response}", endpoint, responseBody);
            return responseBody;
        }

        // Public methods
        public Task<string> GetAsync(string endpoint, string queryString = null, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(queryString))
                endpoint = $"{endpoint}?{queryString}";

            return SendAsync(HttpMethod.Get, endpoint, null, cancellationToken);
        }

        public Task<string> PostAsync(string endpoint, object payload, CancellationToken cancellationToken = default)
        {
            return SendAsync(HttpMethod.Post, endpoint, payload, cancellationToken);
        }

        public Task<string> PutAsync(string endpoint, object payload, CancellationToken cancellationToken = default)
        {
            return SendAsync(HttpMethod.Put, endpoint, payload, cancellationToken);
        }

        public Task<string> PostAsyncBatch<T>(string endpoint, IEnumerable<T> payloads, CancellationToken cancellationToken = default)
        {
            return SendAsync(HttpMethod.Post, endpoint, payloads, cancellationToken);
        }
    }
}
