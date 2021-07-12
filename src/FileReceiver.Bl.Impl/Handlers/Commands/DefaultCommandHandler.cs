using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class DefaultCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;

        public DefaultCommandHandler(IBotMessagesService botMessagesService)
        {
            _botMessagesService = botMessagesService;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.Message.From.Id;
            await _botMessagesService.SendNotSupportedAsync(userId, update.Message.Text.GetCommandFromMessage());
        }
    }
}
