using System;

using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;

namespace FileReceiver.Dal.Impl
{
    public class FileReceivingSessionRepository
        : GenericKeyRepository<Guid, FileReceivingSessionEntity, FileReceiverDbContext>,
            IFileReceivingSessionRepository
    {
        public FileReceivingSessionRepository(FileReceiverDbContext context) : base(context)
        {
        }
    }
}
