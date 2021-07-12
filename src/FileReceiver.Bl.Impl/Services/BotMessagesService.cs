using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace FileReceiver.Bl.Impl.Services
{
    public class BotMessagesService : IBotMessagesService
    {
        private readonly ITelegramBotClient _botClient;

        public BotMessagesService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendMenuAsync(long userOrChatId)
        {
            // TODO: Create a real bot menu with buttons which will invoke command handlers and replace plain text with resources file
            await _botClient.SendTextMessageAsync(userOrChatId, "This is a bot menu...");
        }

        public async Task SendErrorAsync(long userOrChatId, string errorMessage)
        {
            // TODO: Update error sending, maybe add an image or smth like that and replace plain text with resources file
            await _botClient.SendTextMessageAsync(userOrChatId,
                $"Sorry, an error happen. Error description: {errorMessage}");
        }

        public async Task SendTextMessageAsync(long userOrChatId, string message)
        {
            await _botClient.SendTextMessageAsync(userOrChatId, message);
        }

        public async Task SendNotSupportedAsync(long userOrChatId, string notSupportedActionName)
        {
            await _botClient.SendTextMessageAsync(userOrChatId,
                $"Sorry, this action: *{notSupportedActionName}* is not supported now", ParseMode.Markdown);
        }
    }
}
