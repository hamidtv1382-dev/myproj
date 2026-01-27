namespace Review_Rating_Service.src._01_Domain.Core.Events
{
    public class ReviewCreatedEvent
    {
        public Guid ReviewId { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public DateTime OccurredOn { get; set; }

        public ReviewCreatedEvent(Guid reviewId, Guid productId, Guid userId)
        {
            ReviewId = reviewId;
            ProductId = productId;
            UserId = userId;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
