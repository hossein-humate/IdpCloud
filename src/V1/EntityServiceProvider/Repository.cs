using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataProvider.DatabaseContext;
using Entity;
using General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityServiceProvider
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly EfCoreContext Context;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly IConfigurationProvider MapperProvider;
        protected readonly IMapper Mapper;

        protected Repository(EfCoreContext context, IMapper mapper)
        {
            Context = context;
            MapperProvider = mapper.ConfigurationProvider;
            Mapper = mapper;
            DbSet = context.Set<TEntity>();
            QueryableContextWithDeleted = context.Set<TEntity>();
            QueryableContext = context.Set<TEntity>().Where(t => t.DeleteDate == null);
            QueryableContextAsNoTracking = context.Set<TEntity>()
                .AsNoTracking().Where(t => t.DeleteDate == null);
        }

        public void SaveChange()
        {
            Context.SaveChanges();
        }

        public async Task SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<TEntity> QueryableContext { get; set; }
        public IQueryable<TEntity> QueryableContextAsNoTracking { get; }
        public IQueryable<TEntity> QueryableContextWithDeleted { get; }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return QueryableContext.Any(predicate);
        }

        public virtual bool AnyAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return QueryableContextAsNoTracking.Any(predicate);
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContext, includes).FirstOrDefault(predicate);
        }

        public TEntity FindAsNoTracking(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContextAsNoTracking, includes).FirstOrDefault(predicate);
        }

        public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContext.Where(predicate), includes).ToList();
        }

        public virtual IEnumerable<TEntity> FindAllAsNoTracking(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContextAsNoTracking.Where(predicate), includes).ToList();
        }

        public virtual void Update(TEntity entity)
        {
            entity.UpdateDate = DateTime.Now.ConvertToTimestamp();
            DbSet.Update(entity);
        }

        public virtual void DeletePhysical(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void DeletePhysicalRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = QueryableContext.FirstOrDefault(predicate);
            if (entity == null)
            {
                return;
            }

            entity.DeleteDate = DateTime.Now.ConvertToTimestamp();
            DbSet.Update(entity);
        }

        public virtual void DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = QueryableContext.Where(predicate);
            if (!entities.Any())
            {
                return;
            }

            foreach (var entity in entities)
            {
                entity.DeleteDate = DateTime.Now.ConvertToTimestamp();
            }
            DbSet.UpdateRange(entities);
        }

        public virtual void Add(TEntity entity)
        {
            entity.UpdateDate = DateTime.Now.ConvertToTimestamp();
            entity.CreateDate = DateTime.Now.ConvertToTimestamp();
            DbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await QueryableContext.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await QueryableContext.AnyAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await QueryableContextAsNoTracking.AnyAsync(predicate, cancellationToken);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContext, includes).FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<TEntity> FindAsNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContextAsNoTracking, includes)
.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContext.Where(predicate), includes).ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsNoTrackingAsync(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContextAsNoTracking.Where(predicate), includes)
.ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(TEntity entity,
            CancellationToken cancellationToken = default)
        {
            entity.UpdateDate = DateTime.Now.ConvertToTimestamp();
            entity.CreateDate = DateTime.Now.ConvertToTimestamp();
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public virtual async Task AddRangeIfNotExistsAsync(IEnumerable<TEntity> entities,
            Func<TEntity, bool> predicate,
            CancellationToken cancellationToken = default)
        {
            await AddRangeIfNotExistsAsync(DbSet, entities, predicate, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await QueryableContext.Where(predicate).CountAsync(cancellationToken);
        }

        public void FromSqlRaw(string sql)
        {
            DbSet.FromSqlRaw(sql);
        }

        #region Extention
        public IQueryable<TEntity> IncludeMultiple(IQueryable<TEntity> query,
            params Expression<Func<TEntity, object>>[] includes)
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) =>
                current.Include(include));
            }

            return query;
        }

        public static EntityEntry<TEntity> AddIfNotExists(DbSet<TEntity> dbSet, TEntity entity,
            Func<TEntity, bool> predicate)
        {
            var exists = dbSet.Any(c => predicate(entity).Equals(predicate(c)));
            return exists
                ? null
                : dbSet.Add(entity);
        }

        public static void AddRangeIfNotExists(DbSet<TEntity> dbSet, IEnumerable<TEntity> entities,
            Func<TEntity, bool> predicate)
        {
            var entitiesExist = dbSet.Where(ent => entities.Any(add => predicate(ent).Equals(predicate(add))));
            dbSet.AddRange(entities.Except(entitiesExist));
        }

        public static async Task AddRangeIfNotExistsAsync(DbSet<TEntity> dbSet,
            IEnumerable<TEntity> entities,
            Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
        {
            var entitiesExist = dbSet.Where(ent => entities.Any(add => predicate(ent).Equals(predicate(add))));
            await dbSet.AddRangeAsync(entities.Except(entitiesExist), cancellationToken);
        }
        #endregion

        #region ProjectTo Use For Mapper

        public virtual void Add<TProject>(TProject project)
        {
            var entity = Mapper.Map<TEntity>(project);
            Add(entity);
        }

        public virtual async Task AddAsync<TProject>(TProject project, CancellationToken cancellationToken = default)
        {
            var entity = Mapper.Map<TEntity>(project);
            await AddAsync(entity, cancellationToken);
        }

        public virtual void Update<TProject>(Expression<Func<TEntity, bool>> predicate, TProject project)
        {
            var entity = QueryableContextAsNoTracking.FirstOrDefault(predicate);
            if (entity == null) return;
            Mapper.Map(project, entity);
            entity.UpdateDate = DateTime.Now.ConvertToTimestamp();
            Update(entity);
        }

        public virtual async Task UpdateAsync<TProject>(Expression<Func<TEntity, bool>> predicate, TProject project,
            CancellationToken cancellationToken = default)
        {
            var entity = await QueryableContextAsNoTracking.FirstOrDefaultAsync(predicate, cancellationToken);
            Mapper.Map(project, entity);
            entity.UpdateDate = DateTime.Now.ConvertToTimestamp();
            Update(entity);
        }

        public virtual TProject Find<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContext.Where(predicate), includes).ProjectTo<TProject>(MapperProvider)
.FirstOrDefault();
        }

        public TProject FindAsNoTracking<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContextAsNoTracking.Where(predicate), includes)
.ProjectTo<TProject>(MapperProvider).FirstOrDefault();
        }

        public virtual IEnumerable<TProject> FindAll<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContext.Where(predicate), includes).ProjectTo<TProject>(MapperProvider)
.ToList();
        }

        public virtual IEnumerable<TProject> FindAllAsNoTracking<TProject>(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return IncludeMultiple(QueryableContextAsNoTracking.Where(predicate), includes)
.ProjectTo<TProject>(MapperProvider).ToList();
        }

        public virtual async Task<TProject> FindAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await QueryableContext.Where(predicate).ProjectTo<TProject>(MapperProvider)
.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TProject> FindAsNoTrackingAsync<TProject>(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContextAsNoTracking.Where(predicate), includes)
.ProjectTo<TProject>(MapperProvider).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TProject>> FindAllAsync<TProject>(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContext.Where(predicate), includes).ProjectTo<TProject>(MapperProvider)
.ToListAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TProject>> FindAllAsNoTrackingAsync<TProject>(
            Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await IncludeMultiple(QueryableContextAsNoTracking.Where(predicate), includes)
.ProjectTo<TProject>(MapperProvider).ToListAsync(cancellationToken);
        }
        #endregion
    }
}