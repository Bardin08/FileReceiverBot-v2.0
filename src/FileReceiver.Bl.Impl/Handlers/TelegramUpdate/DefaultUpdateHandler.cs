using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.TelegramUpdate
{
    public class DefaultUpdateHandler : IUpdateHandler
    {
        private IBotMessagesService _botMessagesService;

        public DefaultUpdateHandler(IBotMessagesService botMessagesService)
        {
            _botMessagesService = botMessagesService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            // TODO: Add better mechanism for receiving userId
            var userId = update.Message.From.Id;

            await _botMessagesService.SendTextMessageAsync(userId, "Sorry, I can`t figure out your request");
        }
    }
}
