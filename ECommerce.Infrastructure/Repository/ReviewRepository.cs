using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Review>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.reviews
                .AsNoTracking()
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Review?> GetByCustomerAndProductAsync(
            string customerId,
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.reviews
                .FirstOrDefaultAsync(
                    r => r.CustomerId == customerId &&
                         r.ProductId == productId,
                    cancellationToken);
        }

        public async Task<double> GetAverageRatingAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.reviews
                .Where(r => r.ProductId == productId)
                .Select(r => (double)r.Rating)
                .DefaultIfEmpty(0)
                .AverageAsync(cancellationToken);
        }
    }
}
