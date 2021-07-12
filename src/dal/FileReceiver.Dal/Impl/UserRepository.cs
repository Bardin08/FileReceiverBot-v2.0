using System.Threading.Tasks;

using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;

using Microsoft.EntityFrameworkCore;

namespace FileReceiver.Dal.Impl
{
    public class UserRepository : GenericKeyRepository<long, UserEntity, FileReceiverDbContext>, IUserRepository
    {
        public UserRepository(FileReceiverDbContext context) : base(context)
        {
        }

        public async Task<bool> CheckIfUserExists(long userId)
        {
            return await Context.Users.AnyAsync(x => x.Id == userId);
        }
    }
}
