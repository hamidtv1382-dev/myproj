using Payment_Service.src._01_Domain.Core.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Payment_Service.src._02_Application.DTOs.Requests
{
    public class MakePaymentRequestDto
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Money Amount { get; set; }

        [Required]
        public string Method { get; set; }
    }
}
