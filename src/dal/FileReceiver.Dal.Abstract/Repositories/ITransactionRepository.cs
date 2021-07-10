using System;
using System.Threading.Tasks;

using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

namespace FileReceiver.Dal.Abstract.Repositories
{
    public interface ITransactionRepository : IGenericKeyRepository<Guid, TransactionEntity>
    {
        Task<TransactionEntity> GetByUserIdAsync(long userId, TransactionTypeDb transactionType);
        Task<TransactionEntity> GetLastActiveTransactionByUserId(long userId);
    }
}
