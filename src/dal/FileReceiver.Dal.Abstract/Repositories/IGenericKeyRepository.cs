using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileReceiver.Dal.Abstract.Repositories
{
    public interface IGenericKeyRepository<in TKey, TEntity>
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task<TEntity> DeleteAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(TKey id);
        Task<int> GetCountAsync();
        Task<List<TEntity>> PagingFetchAsync(int startIndex, int count);
        Task SaveAsync();
    }
}
