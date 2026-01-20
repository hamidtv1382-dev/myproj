namespace Order_Service.src._02_Application.DTOs.Responses
{
    public class BasketDetailResponseDto
    {
        public Guid Id { get; set; }
        public List<BasketItemResponseDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? AppliedDiscountCode { get; set; }

        public class BasketItemResponseDto
        {
            public Guid ProductId { get; set; }
            public string ProductName { get; set; }
            public string? ImageUrl { get; set; }
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}
