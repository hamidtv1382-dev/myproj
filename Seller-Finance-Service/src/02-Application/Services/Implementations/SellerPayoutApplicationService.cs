using Seller_Finance_Service.src._01_Domain.Core.Interfaces.UnitOfWork;
using Seller_Finance_Service.src._01_Domain.Core.ValueObjects;
using Seller_Finance_Service.src._01_Domain.Services.Interfaces;
using Seller_Finance_Service.src._02_Application.DTOs.Requests;
using Seller_Finance_Service.src._02_Application.DTOs.Responses;
using Seller_Finance_Service.src._02_Application.Exceptions;
using Seller_Finance_Service.src._02_Application.Interfaces;
using Seller_Finance_Service.src._02_Application.Mappings;
using Seller_Finance_Service.src._02_Application.Services.Interfaces;

namespace Seller_Finance_Service.src._02_Application.Services.Implementations
{
    public class SellerPayoutApplicationService : ISellerPayoutApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayoutPolicyService _payoutPolicy;
        private readonly IFinanceService _financeService; // To trigger transfer
        private readonly SellerFinanceMappingProfile _mapper;

        public SellerPayoutApplicationService(IUnitOfWork unitOfWork, IPayoutPolicyService payoutPolicy, IFinanceService financeService, SellerFinanceMappingProfile mapper)
        {
            _unitOfWork = unitOfWork;
            _payoutPolicy = payoutPolicy;
            _financeService = financeService;
            _mapper = mapper;
        }

        public async Task<SellerPayoutResponseDto> RequestPayoutAsync(RequestSellerPayoutDto request)
        {
            var account = await _unitOfWork.SellerAccounts.GetBySellerIdAsync(request.SellerId);
            if (account == null) throw new SellerAccountNotFoundException("Seller account not found.");

            var payout = _payoutPolicy.CreatePayout(account, new Money(request.Amount, "IRR"));

            // Deduct from Available Balance immediately
            account.Balance.DeductFromAvailable(payout.Amount);

            // Mark as processing
            payout.MarkAsProcessing();

            await _unitOfWork.SellerPayouts.AddAsync(payout);
            _unitOfWork.SellerAccounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();

            // Interact with Finance Service for actual transfer (Async/Background ideally)
            var transferSuccess = await _financeService.TransferToSellerAccountAsync(account.BankAccount, payout.Amount);

            if (transferSuccess)
            {
                payout.MarkAsCompleted("GATEWAY-" + Guid.NewGuid());
            }
            else
            {
                payout.MarkAsFailed("Bank transfer failed");
                // Refund back to balance
                account.Balance.ReleaseFromPendingToAvailable(payout.Amount); // Using a method to add back available
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.MapToPayoutResponseDto(payout);
        }

        public async Task<SellerPayoutResponseDto> GetPayoutStatusAsync(Guid payoutId)
        {
            var payout = await _unitOfWork.SellerPayouts.GetByIdAsync(payoutId);
            if (payout == null) throw new SellerPayoutFailedException("Payout not found.");

            return _mapper.MapToPayoutResponseDto(payout);
        }
    }
}
