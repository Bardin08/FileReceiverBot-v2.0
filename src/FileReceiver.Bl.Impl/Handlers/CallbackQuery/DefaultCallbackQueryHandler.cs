using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.CallbackQuery
{
    public class DefaultCallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly ILogger<DefaultCallbackQueryHandler> _logger;

        public DefaultCallbackQueryHandler(
            IBotMessagesService botMessagesService,
            ILogger<DefaultCallbackQueryHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _logger = logger;
        }

        public async Task HandleCallback(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            var userId = new Update()
            {
                CallbackQuery = callbackQuery
            }.GetTgUserId();

            _logger.LogDebug("Invalid callback query received. Callback query {query}", callbackQuery);
            await _botMessagesService.SendNotSupportedAsync(userId, "This button press can't be handled!");
        }
    }
}
