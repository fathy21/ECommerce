using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IReadOnlyList<Review>> GetByProductIdAsync( Guid productId,CancellationToken cancellationToken = default);

        Task<Review?> GetByCustomerAndProductAsync(string customerId,Guid productId,CancellationToken cancellationToken = default);

        Task<double> GetAverageRatingAsync(Guid productId, CancellationToken cancellationToken = default);
    }
}
