using System;

using FileReceiver.Dal.Entities;

namespace FileReceiver.Dal.Abstract.Repositories
{
    public interface IUserRepository : IGenericKeyRepository<long, UserEntity>
    {
    }
}
