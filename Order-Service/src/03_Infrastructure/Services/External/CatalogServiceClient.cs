using Polly;
using Polly.Retry;

namespace Order_Service.src._03_Infrastructure.Services.External
{
    public class CatalogServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public CatalogServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<bool> ValidateProductAsync(Guid productId)
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.GetAsync($"/api/v1/products/{productId}/validate"));

            return response.IsSuccessStatusCode;
        }

        public async Task<Dictionary<string, object>?> GetProductPricingAsync(Guid productId)
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.GetAsync($"/api/v1/products/{productId}/pricing"));

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        }
    }
}
