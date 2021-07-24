using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.TelegramUpdate
{
    public class DefaultUpdateHandler : IUpdateHandler
    {
        private readonly IBotMessagesService _botMessagesService;

        public DefaultUpdateHandler(IBotMessagesService botMessagesService)
        {
            _botMessagesService = botMessagesService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var userId = update.GetTgUserId();

            await _botMessagesService.SendTextMessageAsync(userId, "Sorry, I can`t figure out your request");
        }
    }
}
