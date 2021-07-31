using System;
using System.Linq;
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

        public async Task<bool> CheckIfTransactionForUserExists(long userId, TransactionTypeDb transactionType,
            TransactionStateDb transactionState)
        {
            return await Context.Transactions.AnyAsync(transaction => transaction.UserId == userId
                                                            && transaction.TransactionType == transactionType
                                                            && transaction.TransactionState == transactionState);
        }

        public async Task<TransactionEntity> GetByUserIdAsync(long userId, TransactionTypeDb transactionType)
        {
            return await Context.Transactions
                .Where(transaction => transaction.UserId == userId
                                               && transaction.TransactionType == transactionType
                                               && transaction.TransactionState == TransactionStateDb.Active)
                .Include(x => x.User)
                .SingleOrDefaultAsync();
        }

        public async Task<TransactionEntity> GetCompletedTransactionByUserIdAsync(long userId,
            TransactionTypeDb transactionType)
        {
            return await Context.Transactions
                .FirstOrDefaultAsync(transaction => transaction.UserId == userId
                                                    && transaction.TransactionType == transactionType
                                                    && transaction.TransactionState == TransactionStateDb.Committed);
        }

        public async Task<TransactionEntity> GetLastActiveTransactionByUserId(long userId)
        {
            return await Context.Transactions
                .FirstOrDefaultAsync(transaction => transaction.UserId == userId
                                                     && transaction.TransactionState == TransactionStateDb.Active);
        }

        public async Task AbortAllTransactionsForUser(long userId)
        {
            var sql = $@"UPDATE ""Transactions"" AS T
                       SET ""TransactionState"" = 'Aborted'
                       WHERE T.""UserId"" = '{userId}' AND T.""TransactionState"" = 'Active'";
            await Context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
