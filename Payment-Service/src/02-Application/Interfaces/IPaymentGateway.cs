using Payment_Service.src._01_Domain.Core.Entities;

namespace Payment_Service.src._02_Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<GatewayResult> ProcessPaymentAsync(Payment payment);
        Task<GatewayVerificationResult> VerifyPaymentAsync(string transactionNumber);
        Task<GatewayResult> ProcessRefundAsync(Refund refund);
    }

    public class GatewayResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }

    public class GatewayVerificationResult
    {
        public bool IsVerified { get; set; }
        public string TransactionId { get; set; }
    }
}
