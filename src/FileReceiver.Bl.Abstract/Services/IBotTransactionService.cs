using System.Threading.Tasks;

using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IBotTransactionService
    {
        /// <summary>
        /// Adds the given transaction to the database.
        /// </summary>
        /// <param name="transaction">Transaction's model which will be converted to entity.</param>
        /// <returns><see cref="Task"/></returns>
        Task<TransactionModel> Add(TransactionModel transaction);

        /// <summary>
        /// Creates a transaction with the specified type for user.
        /// </summary>
        /// <param name="userId">Transaction's owner</param>
        /// <param name="transactionType"><see cref="TransactionType">Type</see> of the transaction which will be completed</param>
        /// <param name="data"><see cref="TransactionModel"/> the represents some metadata about the transaction.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task<TransactionModel> Create(long userId, TransactionType transactionType, TransactionDataModel data = null);

        /// <summary>
        /// Receives the last active transaction with a given type for user.
        /// </summary>
        /// <param name="userId">Transaction's owner</param>
        /// <param name="transactionType"><see cref="TransactionType">Type</see> of the transaction which will be completed</param>
        /// <exception cref="TransactionNotFoundException">Throws when transaction with this type is not exists for user or it's not active</exception>
        /// <returns><see cref="Task{TransactionModel}"/></returns>
        Task<TransactionModel> Get(long userId, TransactionType? transactionType = null);

        /// <summary>
        /// Receives the last active transaction with a given type for user.
        /// </summary>
        /// <param name="userId">Transaction's owner</param>
        /// <param name="transactionType"><see cref="TransactionType">Type</see> of the transaction which will be completed</param>
        /// <exception cref="TransactionNotFoundException">Throws when transaction with this type is not exists for user or it's not active</exception>
        /// <returns><see cref="Task{TransactionModel}"/></returns>
        Task<TransactionModel> GetNullIfNotExists(long userId, TransactionType? transactionType = null);

        /// <summary>
        /// Updates the last active transaction with a given type for user.
        /// </summary>
        /// <exception cref="TransactionNotFoundException">Throws when transaction with this type is not exists for user or it's not active</exception>
        /// <returns><see cref="Task"/></returns>
        Task Update(TransactionModel transaction);

        /// <summary>
        /// Updates the last active transaction's state with a given type for user.
        /// </summary>
        /// <param name="userId">Transaction's owner</param>
        /// <param name="transactionType"><see cref="TransactionType">Type</see> of the transaction which will be completed</param>
        /// <param name="transactionState"><see cref="TransactionState">State</see> of the transaction</param>
        /// <exception cref="TransactionNotFoundException">Throws when transaction with this type is not exists for user or it's not active</exception>
        /// <returns><see cref="Task"/></returns>
        Task UpdateState(long userId, TransactionType transactionType, TransactionState transactionState);

        /// <summary>
        /// Completes the last active transaction by user's Id.
        /// </summary>
        /// <param name="userId">Transaction's owner</param>
        /// <param name="transactionType"><see cref="TransactionType">Type</see> of the transaction which will be completed</param>
        /// <returns><see cref="Task"/></returns>
        Task Complete(long userId, TransactionType transactionType);

        /// <summary>
        /// Aborts all transactions for user with a given userId.
        /// </summary>
        Task AbortAllForUser(long userId);
    }
}
