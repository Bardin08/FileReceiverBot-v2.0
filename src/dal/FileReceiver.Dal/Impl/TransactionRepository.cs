using System;
using System.Threading.Tasks;

using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Microsoft.EntityFrameworkCore;

namespace FileReceiver.Dal.Impl
{
    public class TransactionRepository : GenericKeyRepository<Guid, TransactionEntity, FileReceiverDbContext>,
        ITransactionRepository
    {
        public TransactionRepository(FileReceiverDbContext context) : base(context)
        {
        }

        public async Task<TransactionEntity> GetByUserIdAsync(long userId, TransactionTypeDb transactionType)
        {
            return await Context.Transactions
                .FirstOrDefaultAsync(transaction => transaction.UserId == userId 
                                               && transaction.TransactionType == transactionType
                                               && transaction.TransactionState == TransactionStateDb.Active);
        }

        public async Task<TransactionEntity> GetLastActiveTransactionByUserId(long userId)
        {
            return await Context.Transactions
                .SingleOrDefaultAsync(transaction => transaction.UserId == userId 
                                                     && transaction.TransactionState == TransactionStateDb.Active);
        }
    }
}
