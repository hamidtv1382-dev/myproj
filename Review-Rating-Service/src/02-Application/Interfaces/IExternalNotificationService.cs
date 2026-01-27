namespace Review_Rating_Service.src._02_Application.Interfaces
{
    public interface IExternalNotificationService
    {
        Task NotifySellerOnReviewCreatedAsync(Guid productId, string reviewText);
        Task NotifyAdminOnPendingReviewAsync(Guid reviewId);
    }
}
