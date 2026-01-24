using Finance_Service.src._01_Domain.Core.Entities;
using Finance_Service.src._01_Domain.Core.ValueObjects;

namespace Finance_Service.src._01_Domain.Services.Interfaces
{
    public interface IFinanceDomainService
    {
        Task<Fee> CalculateAndApplyFeeAsync(Guid orderId, Money totalOrderAmount, Guid? sellerId);
        Task<Commission> CalculateCommissionAsync(Guid orderId, Guid sellerId, Money saleAmount);
        Task<Settlement> CreateSellerSettlementAsync(Guid sellerId);
        Task<bool> ProcessSettlementAsync(Guid settlementId);
    }
}
