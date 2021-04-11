using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Services
{
    [Obsolete("Использовать `DbContext` напрямую")]
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity: class
    {
        private readonly MainContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(MainContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public ValueTask<EntityEntry<TEntity>> Create(TEntity entity, CancellationToken token = default) 
        {
            return _dbContext.AddAsync(entity, token);
        }

        public IAsyncEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsAsyncEnumerable();
        }

        public ValueTask<TEntity> Read (Guid guid)
        {
            return _dbSet.FindAsync(guid);
        }

        public ValueTask<TEntity> Read (Guid guid, CancellationToken token)
        {
            return _dbSet.FindAsync(new object[] { guid }, token);
        }

        public ValueTask<TEntity> Read (params object[] keyValues)
        {
            return _dbSet.FindAsync(keyValues);
        }

        public ValueTask<TEntity> Read (object[] keyValues, CancellationToken token)
        {
            return _dbSet.FindAsync(keyValues, token);
        }

        public IAsyncEnumerable<TEntity> Read (params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _dbSet.AsNoTracking();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> Read()
        {
            return _dbSet.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> ReadPage(int count, int pageNumber)
        {
            return _dbSet.Skip((pageNumber - 1) * count).Take(count).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> ReadPage(int count, int pageNumber, Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Skip((pageNumber - 1) * count).Where(predicate).Take(count).AsAsyncEnumerable();
        }

        public void Remove(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
    }
}
