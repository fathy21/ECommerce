using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // Customer creates a review
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateReview(
            CreateReviewDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
                return Unauthorized();

            var review = await _reviewService.CreateAsync(
                customerId, dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetProductReviews),
                new { productId = review.ProductId },
                review);
        }

        // Get all reviews for a product
        [HttpGet("product/{productId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var reviews = await _reviewService.GetByProductIdAsync(
                productId, cancellationToken);

            return Ok(reviews);
        }

        // Get product average rating
        [HttpGet("product/{productId:guid}/average")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAverageRating(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var average = await _reviewService.GetAverageRatingAsync(
                productId, cancellationToken);

            return Ok(new
            {
                ProductId = productId,
                AverageRating = average
            });
        }

        // Admin deletes a review
        [HttpDelete("{reviewId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(
            Guid reviewId,
            CancellationToken cancellationToken)
        {
            var deleted = await _reviewService.DeleteAsync(
                reviewId, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
