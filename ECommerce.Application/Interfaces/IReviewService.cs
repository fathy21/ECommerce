using ECommerce.Application.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> CreateAsync(
            string customerId,
            CreateReviewDto dto,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ReviewResponseDto>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid reviewId,
            CancellationToken cancellationToken = default);

        Task<double> GetAverageRatingAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
