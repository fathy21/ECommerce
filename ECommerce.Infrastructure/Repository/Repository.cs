using ECommerce.Application.Interfaces;
using ECommerce.Domain.Common;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id,CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(
                new object?[] { id },
                cancellationToken);
        }

       public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
       {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);
       }

        public async Task AddAsync(T entity,CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity) 
        {
            _dbSet.Remove(entity);
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(IQueryable<T> query,
         int pageNumber,int pageSize,CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (items, totalCount);
        }
    }

}
