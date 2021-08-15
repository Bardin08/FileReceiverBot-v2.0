using System;
using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Exceptions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Services
{
    public class FileReceivingService : IFileReceivingService
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFileReceivingSessionRepository _fileReceivingSessionRepository;
        private readonly IMapper _mapper;

        public FileReceivingService(
            IBotMessagesService botMessagesService,
            ITransactionRepository transactionRepository,
            IFileReceivingSessionRepository fileReceivingSessionRepository,
            IMapper mapper)
        {
            _botMessagesService = botMessagesService;
            _transactionRepository = transactionRepository;
            _fileReceivingSessionRepository = fileReceivingSessionRepository;
            _mapper = mapper;
        }

        public async Task UpdateFileReceivingState(long userId, FileReceivingState newState)
        {
            var transactionEntity = await _transactionRepository.GetLastActiveTransactionByUserId(userId);

            if (transactionEntity.TransactionType is not TransactionTypeDb.FileSending)
            {
                throw new InvalidTransactionTypeException();
            }

            var transactionData = new TransactionDataModel(transactionEntity.TransactionData);
            transactionData.UpdateParameter(TransactionDataParameter.FileReceivingState, newState);
            transactionEntity.TransactionData = transactionData.ParametersAsJson;

            await _transactionRepository.UpdateAsync(transactionEntity);
        }

        public async Task FinishReceivingTransaction(long userId)
        {
            var transactionEntity = await _transactionRepository.GetLastActiveTransactionByUserId(userId);
            if (transactionEntity.TransactionType is not TransactionTypeDb.FileSending)
            {
                return;
            }

            transactionEntity.TransactionState = TransactionStateDb.Committed;
            await _transactionRepository.UpdateAsync(transactionEntity);
        }

        public async Task<bool> SaveDocument(Document document)
        {
            // TODO: Add saving the document to the datasource
            await Task.Delay(5000);
            return true;
        }

        public async Task<bool> SavePhoto(PhotoSize photoSize)
        {
            // TODO: Add saving the photo to the datasource
            await Task.Delay(5000);
            return true;
        }

        public async Task<FileReceivingState> GetFileReceivingStateForUser(long userId)
        {
            try
            {
                var transaction = await GetTransactionAndNotifyIfItsTypeInvalid(userId);

                return transaction.TransactionData.GetDataPiece<FileReceivingState>(
                    TransactionDataParameter.FileReceivingState);
            }
            catch (InvalidTransactionTypeException)
            {
                await _botMessagesService.SendErrorAsync(userId, "An error occured while processing you input. " +
                                                                 "Try to use command /cancel and then start " +
                                                                 "a file sending process again");
                return await Task.FromResult(FileReceivingState.None);
            }
        }

        public async Task<bool> CheckIfSessionExists(Guid token)
        {
            var session = await _fileReceivingSessionRepository.GetByIdAsync(token);
            return session is not null;
        }

        // TODO: maybe create a separate service for actioning with transactions?
        private async Task<TransactionModel> GetTransactionAndNotifyIfItsTypeInvalid(long userId)
        {
            var transaction = _mapper.Map<TransactionModel>(
                await _transactionRepository.GetLastActiveTransactionByUserId(userId));
            if (transaction.TransactionType is not TransactionType.FileSending)
            {
                throw new InvalidTransactionTypeException();
            }

            return transaction;
        }
    }
}
