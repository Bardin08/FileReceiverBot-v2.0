using System;

using FileReceiver.Dal.Entities;

namespace FileReceiver.Dal.Abstract.Repositories
{
    public interface IFileReceivingSessionRepository : IGenericKeyRepository<Guid, FileReceivingSessionEntity>
    {
    }
}
