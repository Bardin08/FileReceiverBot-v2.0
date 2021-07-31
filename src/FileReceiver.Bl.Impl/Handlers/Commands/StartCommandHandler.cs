using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Abstract.Repositories;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class StartCommandHandler : ICommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly IBotMessagesService _botMessagesService;

        public StartCommandHandler(
            IUserRepository userRepository,
            ICommandHandlerFactory commandHandlerFactory,
            IBotMessagesService botMessagesService)
        {
            _userRepository = userRepository;
            _commandHandlerFactory = commandHandlerFactory;
            _botMessagesService = botMessagesService;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();

            var userAccount = await _userRepository.GetByIdAsync(userId);
            if (userAccount == null)
            {
                // TODO: Replace plain text literal with resource file
                var registrationHandler = _commandHandlerFactory.CreateCommandHandler("/register");
                await registrationHandler.HandleCommandAsync(update);
                return;
            }

            await _botMessagesService.SendMenuAsync(userId);
        }
    }
}
