using Microsoft.AspNetCore.Mvc;
using Review_Rating_Service.src._02_Application.DTOs.Requests;
using Review_Rating_Service.src._02_Application.DTOs.Responses;
using Review_Rating_Service.src._02_Application.Services.Implementations;
using Review_Rating_Service.src._02_Application.Services.Interfaces;

namespace Review_Rating_Service.src._04_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewApplicationService _reviewService;

        public ReviewsController(IReviewApplicationService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> CreateReview([FromBody] CreateReviewRequestDto request)
        {
            var result = await _reviewService.CreateReviewAsync(request);
            return CreatedAtAction(nameof(GetReviews), new { productId = result.ProductId }, result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ReviewResponseDto>>> GetReviews([FromQuery] GetReviewsFilterRequestDto filter)
        {
            var result = await _reviewService.GetReviewsAsync(filter);
            return Ok(result);
        }

        [HttpGet("summary/{productId}")]
        public async Task<ActionResult<ReviewSummaryResponseDto>> GetSummary(Guid productId)
        {
            var result = await _reviewService.GetProductSummaryAsync(productId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewResponseDto>> UpdateReview(Guid id, [FromBody] UpdateReviewRequestDto request)
        {
            if (id != request.ReviewId)
            {
                return BadRequest("ID mismatch.");
            }

            var result = await _reviewService.UpdateReviewAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReview(Guid id)
        {
            await _reviewService.ApproveReviewAsync(id);
            return Ok();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectReview(Guid id)
        {
            await _reviewService.RejectReviewAsync(id);
            return Ok();
        }
    }
}
