using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class DefaultCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly ILogger<DefaultCommandHandler> _logger;

        public DefaultCommandHandler(
            IBotMessagesService botMessagesService,
            ILogger<DefaultCommandHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("A command {commandText} from {userId} wasn't recognized and default handler was invoked!",
                update.Message.Text.GetCommandFromMessage(), userId);
            await _botMessagesService.SendNotSupportedAsync(userId, update.Message.Text.GetCommandFromMessage());
        }
    }
}
