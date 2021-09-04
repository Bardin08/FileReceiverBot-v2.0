using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class SendFileCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserService _userService;
        private readonly IBotTransactionService _transactionService;
        private readonly ILogger<SendFileCommandHandler> _logger;

        public SendFileCommandHandler(
            IBotMessagesService botMessagesService,
            IUserService userService,
            IBotTransactionService transactionService,
            ILogger<SendFileCommandHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _userService = userService;
            _transactionService = transactionService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Send file command received from {userId}", userId);

            if (!await _userService.CheckIfExists(userId))
            {
                _logger.LogWarning("User has some other active transaction, thus file receiving command from {userId} " +
                                   "won't be handled!", userId);
                await _botMessagesService.SendErrorAsync(userId,
                    "You should register before you can use this command, to do this you can use command /register");
                return;
            }

            var transaction = await _transactionService.Create(userId, TransactionType.FileSending);
            transaction.TransactionData.AddDataPiece(TransactionDataParameter.FileReceivingState,
                FileReceivingState.TokenReceived);

            await _transactionService.Add(transaction);
            await _botMessagesService.SendTextMessageAsync(userId, "Okay, send me the session token");
        }
    }
}
