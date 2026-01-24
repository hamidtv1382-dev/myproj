using Payment_Service.src._01_Domain.Core.Enums;
using Payment_Service.src._01_Domain.Core.Interfaces.UnitOfWork;
using Payment_Service.src._01_Domain.Services.Interfaces;
using Payment_Service.src._02_Application.DTOs.Requests;
using Payment_Service.src._02_Application.DTOs.Responses;
using Payment_Service.src._02_Application.Exceptions;
using Payment_Service.src._02_Application.Interfaces;
using Payment_Service.src._02_Application.Mappings;
using Payment_Service.src._02_Application.Services.Interfaces;

namespace Payment_Service.src._02_Application.Services.Implementations
{
    public class PaymentApplicationService : IPaymentApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentDomainService _paymentDomainService;
        private readonly IPaymentGateway _paymentGateway;
        private readonly PaymentMappingProfile _mapper;

        public PaymentApplicationService(IUnitOfWork unitOfWork, IPaymentDomainService paymentDomainService, IPaymentGateway paymentGateway, PaymentMappingProfile mapper)
        {
            _unitOfWork = unitOfWork;
            _paymentDomainService = paymentDomainService;
            _paymentGateway = paymentGateway;
            _mapper = mapper;
        }

        public async Task<PaymentResponseDto> MakePaymentAsync(MakePaymentRequestDto request)
        {
            var payment = await _paymentDomainService.InitiatePaymentAsync(request.OrderId, request.Amount, request.Method);

            await _unitOfWork.Payments.AddAsync(payment);

            var gatewayResult = await _paymentGateway.ProcessPaymentAsync(payment);

            if (gatewayResult.Success)
            {
                _paymentDomainService.CompletePayment(payment, gatewayResult.TransactionId);
            }
            else
            {
                _paymentDomainService.FailPayment(payment, gatewayResult.Message);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.MapToPaymentResponseDto(payment);
        }

        public async Task<PaymentResponseDto> GetPaymentStatusAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null) throw new PaymentFailedException("Payment not found.");

            return _mapper.MapToPaymentResponseDto(payment);
        }

        public async Task<PaymentResponseDto> VerifyPaymentAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null) throw new PaymentFailedException("Payment not found.");

            var verificationResult = await _paymentGateway.VerifyPaymentAsync(payment.TransactionNumber.Value);

            if (verificationResult.IsVerified && payment.Status == PaymentStatus.Pending)
            {
                _paymentDomainService.CompletePayment(payment, verificationResult.TransactionId);
                await _unitOfWork.SaveChangesAsync();
            }
            else if (!verificationResult.IsVerified && payment.Status == PaymentStatus.Pending)
            {
                _paymentDomainService.FailPayment(payment, "Verification failed");
                await _unitOfWork.SaveChangesAsync();
            }

            return _mapper.MapToPaymentResponseDto(payment);
        }
    }
}
