using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetByIdWithItemsAsync(Guid id,CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(string customerId,CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Order>> GetAllWithItemsAsync(CancellationToken cancellationToken = default);
    }
}
