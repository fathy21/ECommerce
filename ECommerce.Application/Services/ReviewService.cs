using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(
            IReviewRepository reviewRepository,
            IRepository<Product> productRepository,
            IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewResponseDto> CreateAsync(
            string customerId,
            CreateReviewDto dto,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                dto.ProductId, cancellationToken);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            var existingReview =
                await _reviewRepository.GetByCustomerAndProductAsync(
                    customerId, dto.ProductId, cancellationToken);

            if (existingReview is not null)
                throw new InvalidOperationException(
                    "You have already reviewed this product.");

            var review = new Review
            {
                CustomerId = customerId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _reviewRepository.AddAsync(review, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToResponse(review);
        }

        public async Task<IReadOnlyList<ReviewResponseDto>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(
                productId, cancellationToken);

            return reviews.Select(MapToResponse).ToList();
        }

        public async Task<bool> DeleteAsync(
            Guid reviewId,
            CancellationToken cancellationToken = default)
        {
            var review = await _reviewRepository.GetByIdAsync(
                reviewId, cancellationToken);

            if (review is null)
                return false;

            _reviewRepository.Delete(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<double> GetAverageRatingAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                productId, cancellationToken);

            if (product is null)
                throw new KeyNotFoundException("Product not found.");

            return await _reviewRepository.GetAverageRatingAsync(
                productId, cancellationToken);
        }

        private static ReviewResponseDto MapToResponse(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                CustomerId = review.CustomerId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }
    }
}
