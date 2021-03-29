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
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity: class
    {
        private readonly MainContext _dbContext;
        public GenericRepository(MainContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask<EntityEntry<TEntity>> Create(TEntity entity, CancellationToken token = default) 
        {
            return _dbContext.AddAsync(entity, token);
        }

        public IAsyncEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate).AsAsyncEnumerable();
        }

        public ValueTask<TEntity> Read (Guid guid)
        {
            return _dbContext.Set<TEntity>().FindAsync(guid);
        }

        public ValueTask<TEntity> Read (Guid guid, CancellationToken token)
        {
            return _dbContext.Set<TEntity>().FindAsync(new object[] { guid }, token);
        }

        public ValueTask<TEntity> Read (params object[] keyValues)
        {
            return _dbContext.Set<TEntity>().FindAsync(keyValues);
        }

        public ValueTask<TEntity> Read (object[] keyValues, CancellationToken token)
        {
            return _dbContext.Set<TEntity>().FindAsync(keyValues, token);
        }

        public IAsyncEnumerable<TEntity> Read (params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _dbContext.Set<TEntity>().AsNoTracking();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> Read()
        {
            return _dbContext.Set<TEntity>().AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> ReadPage(int count, int pageNumber)
        {
            return _dbContext.Set<TEntity>().Skip((pageNumber - 1) * count).Take(count).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TEntity> ReadPage(int count, int pageNumber, Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Skip((pageNumber - 1) * count).Where(predicate).Take(count).AsAsyncEnumerable();
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
            return await _dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken).ConfigureAwait(false);
        }
    }
}
