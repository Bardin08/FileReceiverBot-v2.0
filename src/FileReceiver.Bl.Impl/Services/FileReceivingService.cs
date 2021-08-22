using System;
using System.IO;
using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Exceptions;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Integrations.Mega.Abstract;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Services
{
    public class FileReceivingService : IFileReceivingService
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IBotTransactionService _botTransactionService;
        private readonly IBotTransactionService _transactionService;
        private readonly IFileReceivingSessionRepository _fileReceivingSessionRepository;
        private readonly IMegaApiClient _megaClient;
        private readonly ITelegramBotClient _botClient;

        public FileReceivingService(
            IBotMessagesService botMessagesService,
            IBotTransactionService botTransactionService,
            IBotTransactionService transactionService,
            IFileReceivingSessionRepository fileReceivingSessionRepository,
            IMegaApiClient megaClient,
            ITelegramBotClient botClient)
        {
            _botMessagesService = botMessagesService;
            _botTransactionService = botTransactionService;
            _transactionService = transactionService;
            _fileReceivingSessionRepository = fileReceivingSessionRepository;
            _megaClient = megaClient;
            _botClient = botClient;
        }

        public async Task UpdateFileReceivingState(long userId, FileReceivingState newState)
        {
            var transaction = await _transactionService.Get(userId, TransactionType.FileSending);

            transaction.TransactionData.UpdateParameter(TransactionDataParameter.FileReceivingState, newState);

            await _transactionService.Update(transaction);
        }

        public async Task FinishReceivingTransaction(long userId)
        {
            await _transactionService.UpdateState(userId, TransactionType.FileSending, TransactionState.Committed);
        }

        public async Task<bool> SaveDocument(long userId, Guid sessionId, Document document)
        {
            // TODO: Add constraints check
            var transaction = await _transactionService.Get(userId, TransactionType.FileSending);
            var memStream = new MemoryStream();
            var fileInfo = await _botClient.GetFileAsync(document.FileId);
            await _botClient.DownloadFileAsync(fileInfo.FilePath, memStream);
            var megaResponse = await _megaClient.UploadFile(
                transaction.Id,
                sessionId.ToString(),
                document.FileName,
                memStream);

            return megaResponse.Successful;
        }

        public async Task<FileReceivingState> GetFileReceivingStateForUser(long userId)
        {
            try
            {
                var transaction = await _botTransactionService.Get(userId, TransactionType.FileSending);

                return transaction.TransactionData.GetDataPiece<FileReceivingState>(
                    TransactionDataParameter.FileReceivingState);
            }
            catch (InvalidTransactionTypeException)
            {
                // TODO: Log exception
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
    }
}
