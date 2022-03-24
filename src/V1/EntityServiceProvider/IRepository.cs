using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using  Entity;

namespace EntityServiceProvider
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        void SaveChange();
        Task SaveChangeAsync(CancellationToken cancellationToken = default);
        IQueryable<TEntity> QueryableContext { get; }
        IQueryable<TEntity> QueryableContextAsNoTracking { get; }
        IQueryable<TEntity> QueryableContextWithDeleted { get; }
        bool Any(Expression<Func<TEntity, bool>> predicate);
        bool AnyAsNoTracking(Expression<Func<TEntity, bool>> predicate);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes);

        TEntity FindAsNoTracking(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TEntity> FindAllAsNoTracking(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        void Update(TEntity entity);

        void DeletePhysical(TEntity entity);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        void DeleteRange(Expression<Func<TEntity, bool>> predicate);

        void DeletePhysicalRange(IEnumerable<TEntity> entities);

        void Add(TEntity entity);

        void AddRange(IEnumerable<TEntity> entities);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        Task<bool> AnyAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> FindAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> FindAllAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default);

        Task AddRangeIfNotExistsAsync(IEnumerable<TEntity> entities,
            Func<TEntity, bool> predicate,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        void FromSqlRaw(string sql);

        #region ProjectTo Use For Mapper
        void Add<TProject>(TProject project);

        Task AddAsync<TProject>(TProject project, CancellationToken cancellationToken = default);

        void Update<TProject>(Expression<Func<TEntity, bool>> predicate, TProject project);

        Task UpdateAsync<TProject>(Expression<Func<TEntity, bool>> predicate, TProject project,
            CancellationToken cancellationToken = default);

        TProject Find<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        TProject FindAsNoTracking<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TProject> FindAll<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        IEnumerable<TProject> FindAllAsNoTracking<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TProject> FindAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<TProject> FindAsNoTrackingAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TProject>> FindAllAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TProject>> FindAllAsNoTrackingAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        #endregion
    }
}