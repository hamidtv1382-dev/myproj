using System.ComponentModel.DataAnnotations;

namespace Order_Service.src._02_Application.DTOs.Requests
{
    public class RemoveItemFromBasketRequestDto
    {
        [Required]
        public Guid ProductId { get; set; }
    }
}
