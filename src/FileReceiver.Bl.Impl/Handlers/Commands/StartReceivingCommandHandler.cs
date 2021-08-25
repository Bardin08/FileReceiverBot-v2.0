using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class StartReceivingCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserService _userService;
        private readonly IFileReceivingSessionService _receivingSessionService;
        private readonly ILogger<StartReceivingCommandHandler> _logger;

        public StartReceivingCommandHandler(
            IBotMessagesService botMessagesService,
            IUserService userService,
            IFileReceivingSessionService receivingSessionService,
            ILogger<StartReceivingCommandHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _userService = userService;
            _receivingSessionService = receivingSessionService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Start receiving command received from {userid}", userId);

            if (!await _userService.CheckIfExists(userId))
            {
                _logger.LogWarning("Start receiving command from {userId} won't be processed " +
                                   "because he isn't registered!", userId);
                await _botMessagesService.SendErrorAsync(userId, "Your profile not found. You should register first!");
            }

            await _receivingSessionService.CreateFileReceivingSessionAsync(userId);
            await _botMessagesService.SendTextMessageAsync(userId,
                "First, you should setup all the required constraints. To begin with, " +
                "send me the file max size in bytes");
        }
    }
}
