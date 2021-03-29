using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Interfaces
{
    public interface IGenericRepository
    {
        ValueTask<EntityEntry<TEntity>> Create<TEntity>(TEntity entity, CancellationToken token = default) where TEntity : class;
        ValueTask<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class;
        IAsyncEnumerable<TEntity> Read<TEntity>() where TEntity : class;
        IAsyncEnumerable<TEntity> Read<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        ValueTask<TEntity> Read<TEntity>(Guid guid) where TEntity : class;
        ValueTask<TEntity> Read<TEntity>(Guid guid, CancellationToken token) where TEntity : class;
        ValueTask<TEntity> Read<TEntity>(object[] keyValues, CancellationToken token) where TEntity : class;
        IAsyncEnumerable<TEntity> Read<TEntity>(params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class;
        ValueTask<TEntity> Read<TEntity>(params object[] keyValues) where TEntity : class;
        IAsyncEnumerable<TEntity> ReadPage<TEntity>(int count, int pageNumber) where TEntity : class;
        IAsyncEnumerable<TEntity> ReadPage<TEntity>(int count, int pageNumber, Expression<Func<TEntity, bool>> predicate) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        ValueTask SaveChangesAsync(CancellationToken cancellationToken = default);
        void Update<TEntity>(TEntity entity);
    }
}