using System;
using System.Linq;
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

        public FileSendingUpdateHandler(
            IFileReceivingService fileReceivingService,
            IBotMessagesService botMessagesService)
        {
            _fileReceivingService = fileReceivingService;
            _botMessagesService = botMessagesService;
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
            if (data.Update.Message.Document is not null)
            {
                var isDocumentSaved = await _fileReceivingService.SaveDocument(data.Update.Message.Document);

                if (!isDocumentSaved)
                {
                    await _botMessagesService.SendErrorAsync(data.UserId,
                        "An error occured while saving file. Try again");
                    return;
                }
            }

            if (data.Update.Message.Photo is not null)
            {
                // Telegram always returns 4 photos. The last one is in original quality
                var isPhotoSaved = await _fileReceivingService.SavePhoto(data.Update.Message.Photo.Last());

                if (!isPhotoSaved)
                {
                    await _botMessagesService.SendErrorAsync(data.UserId,
                        "An error occured while photo saving. Try again");
                    return;
                }
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

            if (!await _fileReceivingService.CheckIfSessionExists(token))
            {
                await _botMessagesService.SendErrorAsync(data.UserId, "Session with this token is not exists");
                return false;
            }

            return true;
        }

        private class FileSendingUpdateData
        {
            public long UserId { get; set; }
            public Update Update { get; set; }
        }
    }
}
