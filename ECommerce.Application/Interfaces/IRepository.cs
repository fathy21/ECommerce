using ECommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id , CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);  
        Task AddAsync(T entity , CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> GetQueryable();
        Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(IQueryable<T> query,
        int pageNumber,int pageSize,CancellationToken cancellationToken = default);
    }
}
