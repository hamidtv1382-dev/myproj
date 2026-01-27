using Review_Rating_Service.src._02_Application.Interfaces;

namespace Review_Rating_Service.src._03_Infrastructure.Services.External
{
    public class NotificationClient : IExternalNotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task NotifySellerOnReviewCreatedAsync(Guid productId, string reviewText)
        {
            // Simulate API Call
            var payload = new { ProductId = productId, Text = reviewText };
            await _httpClient.PostAsJsonAsync("/api/notifications/seller/review", payload);
        }

        public async Task NotifyAdminOnPendingReviewAsync(Guid reviewId)
        {
            // Simulate API Call
            var payload = new { ReviewId = reviewId };
            await _httpClient.PostAsJsonAsync("/api/notifications/admin/review", payload);
        }
    }
}
