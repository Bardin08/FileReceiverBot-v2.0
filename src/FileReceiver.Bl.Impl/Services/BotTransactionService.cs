using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

namespace FileReceiver.Bl.Impl.Services
{
    public class BotTransactionService : IBotTransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public BotTransactionService(
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<TransactionModel> Add(TransactionModel transaction)
        {
            await _transactionRepository.AddAsync(_mapper.Map<TransactionEntity>(transaction));
            return transaction;
        }

        public Task<TransactionModel> Create(
            long userId,
            TransactionType transactionType,
            TransactionDataModel data = null)
        {
            return Task.FromResult(new TransactionModel()
            {
                UserId = userId,
                TransactionType = transactionType,
                TransactionState = TransactionState.Active,
                TransactionData = data ?? new TransactionDataModel(),
            });
        }

        public async Task<TransactionModel> Get(long userId, TransactionType? transactionType = null)
        {
            TransactionEntity transactionEnt;
            if (transactionType is not null)
            {
                transactionEnt = await GetTransactionAndThrowExceptionIfNotExists(userId, transactionType.Value);
            }
            else
            {
                transactionEnt = await GetTransactionAndThrowExceptionIfNotExists(userId);
            }

            return _mapper.Map<TransactionModel>(transactionEnt);
        }

        public async Task<TransactionModel> GetNullIfNotExists(long userId, TransactionType? transactionType = null)
        {
            try
            {
                TransactionEntity transactionEnt;
                if (transactionType is not null)
                {
                    transactionEnt = await GetTransactionAndThrowExceptionIfNotExists(userId, transactionType.Value);
                }
                else
                {
                    transactionEnt = await GetTransactionAndThrowExceptionIfNotExists(userId);
                }

                return _mapper.Map<TransactionModel>(transactionEnt);
            }
            catch
            {
                return null;
            }
        }

        public async Task Update(TransactionModel transaction)
        {
            var transactionEnt = await GetTransactionAndThrowExceptionIfNotExists(
                transaction.UserId, transaction.TransactionType);

            transactionEnt.TransactionData = transaction.TransactionData.ParametersAsJson;
            transactionEnt.TransactionState = (TransactionStateDb)transaction.TransactionState;

            await _transactionRepository.UpdateAsync(transactionEnt);
        }

        public async Task UpdateState(long userId, TransactionType transactionType, TransactionState transactionState)
        {
            var transaction = await GetTransactionAndThrowExceptionIfNotExists(userId, transactionType);
            transaction.TransactionState = (TransactionStateDb)transactionState;
            await _transactionRepository.UpdateAsync(transaction);
        }

        public async Task Complete(long userId, TransactionType transactionType)
        {
            await UpdateState(userId, transactionType, TransactionState.Committed);
        }

        public async Task AbortAllForUser(long userId)
        {
            await _transactionRepository.AbortAllTransactionsForUser(userId);
        }

        private async Task<TransactionEntity> GetTransactionAndThrowExceptionIfNotExists(long userId)
        {
            var transactionEnt = await _transactionRepository.GetLastActiveTransactionByUserId(userId);
            if (transactionEnt is null)
            {
                throw new TransactionNotFoundException(userId);
            }

            return transactionEnt;
        }

        private async Task<TransactionEntity> GetTransactionAndThrowExceptionIfNotExists(long userId,
            TransactionType type)
        {
            var transactionEnt = await _transactionRepository.GetByUserIdAsync(userId, (TransactionTypeDb)type);
            if (transactionEnt is null)
            {
                throw new TransactionNotFoundException(userId, type);
            }

            return transactionEnt;
        }
    }
}
