using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class StartCommandHandler : ICommandHandler
    {
        private readonly IUserService _userService;
        private readonly IBotMessagesService _botMessagesService;
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly ILogger<StartCommandHandler> _logger;

        public StartCommandHandler(
            IUserService userService,
            IBotMessagesService botMessagesService,
            ICommandHandlerFactory commandHandlerFactory,
            ILogger<StartCommandHandler> logger)
        {
            _userService = userService;
            _botMessagesService = botMessagesService;
            _commandHandlerFactory = commandHandlerFactory;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Start command received from {userId}", userId);

            var userAccount = await _userService.Get(userId);
            if (userAccount is null)
            {
                var registrationHandler = _commandHandlerFactory.CreateCommandHandler(Common.Constants.Commands.Register);
                await registrationHandler.HandleCommandAsync(update);
                return;
            }

            await _botMessagesService.SendMenuAsync(userId);
        }
    }
}
