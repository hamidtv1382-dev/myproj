using Polly;
using Polly.Retry;

namespace Order_Service.src._03_Infrastructure.Services.External
{
    public class SellerFinanceServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public SellerFinanceServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<bool> NotifySellerAsync(Guid sellerId, Guid orderId, decimal amount)
        {
            var payload = new { SellerId = sellerId, OrderId = orderId, Amount = amount };
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync("/api/v1/seller/notify-sale", payload));

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RequestPayoutAsync(Guid sellerId)
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync("/api/v1/seller/request-payout", new { SellerId = sellerId }));

            return response.IsSuccessStatusCode;
        }
    }
}
