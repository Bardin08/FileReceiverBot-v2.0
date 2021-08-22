using System;
using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.TelegramUpdate
{
    public class FileSendingUpdateHandler : IUpdateHandler
    {
        private readonly IFileReceivingService _fileReceivingService;
        private readonly IBotMessagesService _botMessagesService;
        private readonly IBotTransactionService _transactionService;

        public FileSendingUpdateHandler(
            IFileReceivingService fileReceivingService,
            IBotMessagesService botMessagesService, IBotTransactionService transactionService)
        {
            _fileReceivingService = fileReceivingService;
            _botMessagesService = botMessagesService;
            _transactionService = transactionService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var userId = update.GetTgUserId();

            var data = new FileSendingUpdateData()
            {
                UserId = userId,
                Update = update,
            };

            var fileReceivingState = await _fileReceivingService.GetFileReceivingStateForUser(userId);

            switch (fileReceivingState)
            {
                case FileReceivingState.TokenReceived:
                    await HandleReceivedToken(data);
                    break;
                case FileReceivingState.FileReceived:
                    await HandleReceivedFile(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task HandleReceivedToken(FileSendingUpdateData data)
        {
            if (!await ParseTokenAndCheckIfSessionExists(data))
            {
                return;
            }

            await _botMessagesService.SendTextMessageAsync(data.UserId, "Okay and now send me the file please");
            await _fileReceivingService.UpdateFileReceivingState(data.UserId, FileReceivingState.FileReceived);
        }

        private async Task HandleReceivedFile(FileSendingUpdateData data)
        {
            var transaction = await _transactionService.Get(data.UserId, TransactionType.FileSending);
            if (data.Update.Message.Document is not null)
            {
                var isDocumentSaved = await _fileReceivingService.SaveDocument(
                    data.UserId,
                    new Guid(transaction.TransactionData.GetDataPiece(
                        TransactionDataParameter.FileReceivingSessionId).ToString() ?? Guid.Empty.ToString()),
                    data.Update.Message.Document);

                if (!isDocumentSaved)
                {
                    await _botMessagesService.SendErrorAsync(data.UserId,
                        "An error occured while saving file. Try again");
                    return;
                }
            }

            if (data.Update.Message.Photo is not null)
            {
                await _botMessagesService.SendNotSupportedAsync(data.UserId, "Currently photos saving is not supported. " +
                                                                             "You can save photo as a document");
            }

            await _botMessagesService.SendTextMessageAsync(data.UserId, "Your file saved");
            await _fileReceivingService.UpdateFileReceivingState(data.UserId, FileReceivingState.Complete);
            await _fileReceivingService.FinishReceivingTransaction(data.UserId);
        }

        private async Task<bool> ParseTokenAndCheckIfSessionExists(FileSendingUpdateData data)
        {
            bool isTokenParsed = Guid.TryParse(data.Update.Message.Text, out Guid token);

            if (!isTokenParsed)
            {
                await _botMessagesService.SendErrorAsync(data.UserId, "Invalid token. Try another one");
                return false;
            }

            if (await _fileReceivingService.CheckIfSessionExists(token))
            {
                var transaction = await _transactionService.Get(data.UserId, TransactionType.FileSending);
                transaction.TransactionData.AddDataPiece(TransactionDataParameter.FileReceivingSessionId, token);
                await _transactionService.Update(transaction);
                return true;
            }

            await _botMessagesService.SendErrorAsync(data.UserId, "Session with this token is not exists");
            return false;
        }

        private class FileSendingUpdateData
        {
            public long UserId { get; set; }
            public Update Update { get; set; }
        }
    }
}
