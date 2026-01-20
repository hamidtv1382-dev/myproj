namespace Order_Service.src._01_Domain.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<bool> CheckStockAsync(Guid productId, int quantity);
        Task<bool> ReserveStockAsync(Guid productId, int quantity);
        Task<bool> ReleaseStockAsync(Guid productId, int quantity);
    }
}
