using System.ComponentModel.DataAnnotations;

namespace Order_Service.src._02_Application.DTOs.Requests
{
    public class UpdateOrderRequestDto
    {
        [Required]
        public Guid OrderId { get; set; }

        public string? Description { get; set; }
    }
}
