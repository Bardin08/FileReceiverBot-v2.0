using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileReceiver.Dal.Abstract.Repositories;

using Microsoft.EntityFrameworkCore;

namespace FileReceiver.Dal.Impl
{
    public abstract class GenericKeyRepository<TKey, TEntity, TContext> : IGenericKeyRepository<TKey, TEntity> 
        where TContext: DbContext
        where TEntity: class
    {
        protected GenericKeyRepository(TContext context)
        {
            Context = context;
        }

        protected TContext Context { get; }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var item = await Context.Set<TEntity>().AddAsync(entity);
            await Context.SaveChangesAsync();
            return item.Entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            await Task.FromResult(0);
        }

        public virtual async Task<TEntity> DeleteAsync(TEntity entity)
        {
            TEntity result = Context.Set<TEntity>()
                .Remove(entity).Entity;
            await Context.SaveChangesAsync();
            return result;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await Context.Set<TEntity>()
                .FindAsync(id);
        }

        public virtual async Task<int> GetCountAsync()
        {
            return await Context.Set<TEntity>().CountAsync();
        }

        public virtual async Task<List<TEntity>> PagingFetchAsync(int startIndex, int count)
        {
            return await Context.Set<TEntity>().Skip(startIndex)
                .Take(count).ToListAsync();
        }

        public Task SaveAsync()
        {
            return Context.SaveChangesAsync();
        }
    }
}
