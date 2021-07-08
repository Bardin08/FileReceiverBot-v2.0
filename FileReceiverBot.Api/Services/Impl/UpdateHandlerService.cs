using System.Threading.Tasks;

using FileReceiverBot.Api.Services.Abstract;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileReceiverBot.Api.Services.Impl
{
    public class UpdateHandlerService : IUpdateHandlerService
    {
        private readonly ITelegramBotClient _botClient;

        public UpdateHandlerService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            if (update?.Message != null)
            {
                var updateMessage = update.Message.Text;
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, updateMessage);
            }
        }
    }
}