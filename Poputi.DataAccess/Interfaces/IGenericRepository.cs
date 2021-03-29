using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity: class
    {
        ValueTask<EntityEntry<TEntity>> Create(TEntity entity, CancellationToken token = default);
        ValueTask<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        IAsyncEnumerable<TEntity> Read();
        IAsyncEnumerable<TEntity> Read(Expression<Func<TEntity, bool>> predicate);
        ValueTask<TEntity> Read(Guid guid);
        ValueTask<TEntity> Read(Guid guid, CancellationToken token);
        ValueTask<TEntity> Read(object[] keyValues, CancellationToken token);
        IAsyncEnumerable<TEntity> Read(params Expression<Func<TEntity, object>>[] includeProperties);
        ValueTask<TEntity> Read(params object[] keyValues);
        IAsyncEnumerable<TEntity> ReadPage(int count, int pageNumber);
        IAsyncEnumerable<TEntity> ReadPage(int count, int pageNumber, Expression<Func<TEntity, bool>> predicate);
        void Remove(TEntity entity);
        ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
        void Update(TEntity entity);
    }
}