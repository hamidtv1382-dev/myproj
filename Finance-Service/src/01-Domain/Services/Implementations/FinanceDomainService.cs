using Finance_Service.src._01_Domain.Core.Entities;
using Finance_Service.src._01_Domain.Core.Enums;
using Finance_Service.src._01_Domain.Core.ValueObjects;
using Finance_Service.src._01_Domain.Services.Interfaces;

namespace Finance_Service.src._01_Domain.Services.Implementations
{
    public class FinanceDomainService : IFinanceDomainService
    {
        public async Task<Fee> CalculateAndApplyFeeAsync(Guid orderId, Money totalOrderAmount, Guid? sellerId)
        {
            // Logic: Platform fee is 2% of total order amount
            decimal feeAmount = totalOrderAmount.Amount * 0.02m;
            var fee = new Fee(orderId, new Money(feeAmount, totalOrderAmount.Currency), FeeType.PlatformFee, "Platform Service Fee", sellerId, "System");
            return await Task.FromResult(fee);
        }

        public async Task<Commission> CalculateCommissionAsync(Guid orderId, Guid sellerId, Money saleAmount)
        {
            // Logic: Sales commission is 5% of sale amount
            decimal commissionAmount = saleAmount.Amount * 0.05m;
            var commission = new Commission(orderId, sellerId, new Money(commissionAmount, saleAmount.Currency), CommissionType.SalesCommission, 5.0m, "System");
            return await Task.FromResult(commission);
        }

        public async Task<Settlement> CreateSellerSettlementAsync(Guid sellerId)
        {
            // Logic: Calculate sum of unsettled commissions (simplified here, normally requires repo lookup)
            // For now, creating a placeholder settlement
            var settlement = new Settlement(sellerId, new Money(0, "IRR"), DateTime.UtcNow.AddDays(7), "System");
            return await Task.FromResult(settlement);
        }

        public Task<bool> ProcessSettlementAsync(Guid settlementId)
        {
            // Logic to connect to banking API would go here
            return Task.FromResult(true);
        }
    }
}
