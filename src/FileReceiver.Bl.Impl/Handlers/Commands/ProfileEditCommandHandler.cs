using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Abstract.Repositories;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class ProfileEditCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRepository _userRepository;

        public ProfileEditCommandHandler(IBotMessagesService botMessagesService,
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

            InlineKeyboardMarkup keyboard = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Edit First Name", "profile-edit-first-name"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Edit Last Name", "profile-edit-last-name"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Edit Secret Word", "profile-edit-secret-word"),
                },
            });
            await _botMessagesService.SendMessageWithKeyboardAsync(userId,
                $"Profile info:\n\r\n\rFirst Name: {userModel.FirstName}\n\rLast Name: {userModel.LastName}",
                keyboard);
        }
    }
}
