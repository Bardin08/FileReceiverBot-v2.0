using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Dal.Impl
{
    public class UserRepository : GenericKeyRepository<long, UserEntity, FileReceiverDbContext>, IUserRepository
    {
        public UserRepository(FileReceiverDbContext context) : base(context)
        {
        }
    }
}
