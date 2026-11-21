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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdWithItemsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
            string customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.orders
                .AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDateUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> GetAllWithItemsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.orders
                .AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDateUtc)
                .ToListAsync(cancellationToken);
        }
    }
}
