using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiver.Bl.Impl.Services
{
    public class BotMessagesService : IBotMessagesService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<BotMessagesService> _logger;

        public BotMessagesService(
            ITelegramBotClient botClient,
            ILogger<BotMessagesService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task SendMenuAsync(long userOrChatId)
        {
            // TODO: Create a real bot menu with buttons which will invoke command handlers and replace plain text with resources file
            var sentMsg = await _botClient.SendTextMessageAsync(userOrChatId, "This is a bot menu...");
            if (sentMsg is null) _logger.LogWarning("Message wasn't sent to {userId}", userOrChatId);
        }

        public async Task SendErrorAsync(long userOrChatId, string errorMessage)
        {
            var sentMsg = await _botClient.SendTextMessageAsync(userOrChatId,
                $"Sorry, an error happen. Error description: {errorMessage}");
            if (sentMsg is null) _logger.LogWarning("Message wasn't sent to {userId}", userOrChatId);
        }

        public async Task SendTextMessageAsync(long userOrChatId, string message)
        {
            var sentMsg = await _botClient.SendTextMessageAsync(userOrChatId, message, ParseMode.Markdown);
            if (sentMsg is null) _logger.LogWarning("Message wasn't sent to {userId}", userOrChatId);
        }

        public async Task SendMessageWithKeyboardAsync(long userOrChatId, string message, IReplyMarkup keyboard)
        {
            var sentMsg = await _botClient.SendTextMessageAsync(userOrChatId, message, ParseMode.Markdown, replyMarkup: keyboard);
            if (sentMsg is null) _logger.LogWarning("Message wasn't sent to {userId}", userOrChatId);
        }

        public async Task SendNotSupportedAsync(long userOrChatId, string notSupportedActionName)
        {
            var sentMsg = await _botClient.SendTextMessageAsync(userOrChatId,
                $"Sorry, this action: *{notSupportedActionName}* is not supported now", ParseMode.Markdown);
            if (sentMsg is null) _logger.LogWarning("Message wasn't sent to {userId}", userOrChatId);
        }
    }
}
