using System.Threading.Tasks;

using FileReceiver.Dal.Entities;

namespace FileReceiver.Dal.Abstract.Repositories
{
    public interface IUserRepository : IGenericKeyRepository<long, UserEntity>
    {
        Task<bool> CheckIfUserExistsAsync(long userId);
    }
}
