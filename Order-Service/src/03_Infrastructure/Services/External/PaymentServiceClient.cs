using Polly;
using Polly.Retry;

namespace Order_Service.src._03_Infrastructure.Services.External
{
    public class PaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public PaymentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<PaymentResultDto?> ProcessPaymentAsync(PaymentRequestDto request)
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync("/api/v1/payments/process", request));

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<PaymentResultDto>();
        }

        public async Task<RefundResultDto?> ProcessRefundAsync(RefundRequestDto request)
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync("/api/v1/payments/refund", request));

            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<RefundResultDto>();
        }
    }

    public record PaymentRequestDto(Guid OrderId, decimal Amount, string CallbackUrl, string CardNumber, string Cvv2, string ExpMonth, string ExpYear, string Pin);
    public record PaymentResultDto(Guid TransactionId, bool IsSuccessful, string? Message);
    public record RefundRequestDto(Guid PaymentId, decimal Amount);
    public record RefundResultDto(Guid RefundId, bool IsSuccessful, string? Message);
}
