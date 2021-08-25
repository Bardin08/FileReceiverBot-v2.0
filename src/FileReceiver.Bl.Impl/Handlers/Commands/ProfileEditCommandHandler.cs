using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Constants;
using FileReceiver.Common.Extensions;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class ProfileEditCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserService _userService;
        private readonly ILogger<ProfileEditCommandHandler> _logger;

        public ProfileEditCommandHandler(
            IBotMessagesService botMessagesService,
            IUserService userService,
            ILogger<ProfileEditCommandHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _userService = userService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Profile edit command received from {userId}", userId);

            if (!await _userService.CheckIfExists(userId))
            {
                _logger.LogWarning("Profile command from {userId} won't be handled because he isn't registered yet.",
                    userId);
                await _botMessagesService.SendErrorAsync(userId,
                "You should register before you can use this command, to do this you can use command /register");
                return;
            }

            var user = await _userService.Get(userId);
            await _botMessagesService.SendMessageWithKeyboardAsync(userId,
                $"Profile info:\n\r\n\rFirst Name: {user.FirstName}\n\rLast Name: {user.LastName}",
                Keyboards.ProfileEditActionsKeyboard);
        }
    }
}
