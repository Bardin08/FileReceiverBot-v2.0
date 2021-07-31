using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Common.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.CallbackQuery
{
    public class FileReceivingSessionCallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly IUpdateHandlerFactory _updateHandlerFactory;

        public FileReceivingSessionCallbackQueryHandler(
            IUpdateHandlerFactory updateHandlerFactory)
        {
            _updateHandlerFactory = updateHandlerFactory;
        }

        public async Task HandleCallback(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            switch (callbackQuery)
            {
                case { Data: "fr-session-storage-mega" }:
                    await SetFilesStorageAsync(callbackQuery, FileStorageType.Mega);
                    break;
                case { Data: "fr-session-execute" }:
                    await ExecuteSessionAsync(callbackQuery);
                    break;
                case { Data: "fr-session-cancel" }:
                    await StopSessionAsync(callbackQuery);
                    break;
            }
        }

        private async Task SetFilesStorageAsync(Telegram.Bot.Types.CallbackQuery callbackQuery, FileStorageType storage)
        {
            await _updateHandlerFactory
                .CreateUpdateHandler(TransactionType.FileReceivingSessionCreating)
                .HandleUpdateAsync(new Update()
                {
                    CallbackQuery = callbackQuery,
                    Message = new Message()
                    {
                        Text = storage.ToString(),
                    },
                });
        }

        private async Task ExecuteSessionAsync(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            await _updateHandlerFactory
                .CreateUpdateHandler(TransactionType.FileReceivingSessionCreating)
                .HandleUpdateAsync(new Update()
                {
                    CallbackQuery = callbackQuery,
                });
        }

        private async Task StopSessionAsync(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            await _updateHandlerFactory
                .CreateUpdateHandler(TransactionType.FileReceivingSessionCreating)
                .HandleUpdateAsync(new Update()
                {
                    CallbackQuery = callbackQuery,
                });
        }
    }
}
