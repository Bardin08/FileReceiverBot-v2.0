using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Exceptions;
using FileReceiver.Common.Extensions;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class StartReceivingCommandHandler : ICommandHandler
    {
        private readonly IFileReceivingSessionService _receivingSessionService;
        private readonly IBotMessagesService _botMessagesService;

        public StartReceivingCommandHandler(
            IFileReceivingSessionService receivingSessionService,
            IBotMessagesService botMessagesService)
        {
            _receivingSessionService = receivingSessionService;
            _botMessagesService = botMessagesService;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();

            try
            {
                await _receivingSessionService.CreateFileReceivingSessionAsync(userId);
                await _botMessagesService.SendTextMessageAsync(userId,
                    "First, you should setup all the required constraints. To begin with, " +
                    "send me the file max size in bytes");
            }
            catch (UserProfileNotFoundException ex)
            {
                // TODO: Log the exception
                await _botMessagesService.SendErrorAsync(userId, "Your profile not found. You should register first!");
            }
        }
    }
}
