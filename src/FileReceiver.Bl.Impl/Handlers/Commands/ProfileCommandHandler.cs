using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Abstract.Repositories;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class ProfileCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRepository _userRepository;

        public ProfileCommandHandler(IBotMessagesService botMessagesService,
            IUserRepository userRepository)
        {
            _botMessagesService = botMessagesService;
            _userRepository = userRepository;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            if (!await _userRepository.CheckIfUserExists(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "You should register before you can use this command, to do this you can use command /register");
                return;
            }
            var userModel = await _userRepository.GetByIdAsync(userId);

            await _botMessagesService.SendTextMessageAsync(userId,
                $"Profile info:\n\r\n\rFirst Name: {userModel.FirstName}\n\rLast Name: {userModel.LastName}");
        }
    }
}
