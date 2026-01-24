using System.ComponentModel.DataAnnotations;

namespace Finance_Service.src._02_Application.DTOs.Requests
{
    public class CreateSettlementRequestDto
    {
        [Required]
        public Guid SellerId { get; set; }

        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}
