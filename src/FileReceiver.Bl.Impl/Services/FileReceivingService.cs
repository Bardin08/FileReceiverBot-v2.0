using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Exceptions;
using FileReceiver.Common.Models;
using FileReceiver.Integrations.Mega.Abstract;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Services
{
    public class FileReceivingService : IFileReceivingService
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IBotTransactionService _botTransactionService;
        private readonly IBotTransactionService _transactionService;
        private readonly IFileReceivingSessionService _fileReceivingSessionService;
        private readonly IMegaApiClient _megaClient;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<FileReceivingService> _logger;

        public FileReceivingService(
            IBotMessagesService botMessagesService,
            IBotTransactionService botTransactionService,
            IBotTransactionService transactionService,
            IFileReceivingSessionService fileReceivingSessionService,
            IMegaApiClient megaClient,
            ITelegramBotClient botClient,
            ILogger<FileReceivingService> logger)
        {
            _botMessagesService = botMessagesService;
            _botTransactionService = botTransactionService;
            _transactionService = transactionService;
            _fileReceivingSessionService = fileReceivingSessionService;
            _megaClient = megaClient;
            _botClient = botClient;
            _logger = logger;
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
            var transaction = await _transactionService.Get(userId, TransactionType.FileSending);

            var memStream = new MemoryStream();
            var fileInfo = await _botClient.GetFileAsync(document.FileId);
            await _botClient.DownloadFileAsync(fileInfo.FilePath, memStream);

            var session = await _fileReceivingSessionService.Get(sessionId);

            if (!IsConstraintsCompleted(session, memStream)) return false;

            var megaResponse = await _megaClient.UploadFile(
                transaction.Id,
                sessionId.ToString(),
                document.FileName,
                memStream);

            if (!megaResponse.Successful)
            {
                await _botMessagesService.SendTextMessageAsync(userId,
                    "An error occured while sending saving a file. " +
                    "*Error:* " + megaResponse.FailDescription);
                transaction.TransactionState = TransactionState.Failed;
            }

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
            catch (InvalidTransactionTypeException ex)
            {
                _logger.LogError(ex, "An error occured " + ex.Message + " for user {userId}", userId);
                await _botMessagesService.SendErrorAsync(userId, "An error occured while processing you input. " +
                                                                 "Try to use command /cancel and then start " +
                                                                 "a file sending process again");
                return await Task.FromResult(FileReceivingState.None);
            }
        }

        public async Task<bool> CheckIfSessionExists(Guid sessionId)
        {
            var session = await _fileReceivingSessionService.Get(sessionId);
            return session is not null;
        }

        private bool IsConstraintsCompleted(FileReceivingSessionModel sessionModel, MemoryStream stream)
        {
            var filesAmountConstraint = int.Parse(
                sessionModel.Constrains.GetConstraint(ConstraintType.FileSessionMaxFiles));
            if (sessionModel.FilesReceived >= filesAmountConstraint) return false;

            var fileNamePattern = sessionModel.Constrains.GetConstraint(ConstraintType.FileName);
            if (fileNamePattern != "-1" && !Regex.IsMatch("", fileNamePattern)) return false;

            var fileSizeConstraint = int.Parse(sessionModel.Constrains.GetConstraint(ConstraintType.FileSize));
            if (stream.Length > fileSizeConstraint) return false;

            // TODO: Add a file's extension validation.
            // It's not possible to implement simply with MemoryStream.
            // This operation requires to analyze stream's content

            return false;
        }
    }
}
