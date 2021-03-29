using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Poputi.DataAccess.Services
{
    public class GenericRepository : IGenericRepository
    {
        private readonly MainContext _dbContext;
        public GenericRepository(MainContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask<EntityEntry<TEntity>> Create<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class
        {
            return _dbContext.AddAsync(entity, token);
        }

        public IAsyncEnumerable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return _dbContext.Set<TEntity>().Where(predicate).AsAsyncEnumerable();
        }

        public ValueTask<TEntity> Read<TEntity>(Guid guid) where TEntity : class
        {
            return _dbContext.Set<TEntity>().FindAsync(guid);
        }

        public ValueTask<TEntity> Read<TEntity>(Guid guid, CancellationToken token) where TEntity : class
        {
            return _dbContext.Set<TEntity>().FindAsync(new object[] { guid }, token);
        }

        public ValueTask<TEntity> Read<TEntity>(params object[] keyValues) where TEntity : class
        {
            return _dbContext.Set<TEntity>().FindAsync(keyValues);
        }

        public ValueTask<TEntity> Read<TEntity>(object[] keyValues, CancellationToken token) where TEntity : class
        {
            return _dbContext.Set<TEntity>().FindAsync(keyValues, token);
        }

        public IAsyncEnumerable<TEntity> Read<TEntity>(params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            var query = _dbContext.Set<TEntity>().AsNoTracking();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> Read<TEntity>() where TEntity : class
        {
            return _dbContext.Set<TEntity>().AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> ReadPage<TEntity>(int count, int pageNumber) where TEntity : class
        {
            return _dbContext.Set<TEntity>().Skip((pageNumber - 1) * count).Take(count).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> ReadPage<TEntity>(int count, int pageNumber, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return _dbContext.Set<TEntity>().Skip((pageNumber - 1) * count).Where(predicate).Take(count).AsAsyncEnumerable();
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Remove(entity);
        }

        public void Update<TEntity>(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
    }
}
