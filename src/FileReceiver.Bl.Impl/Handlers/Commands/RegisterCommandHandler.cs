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
    public class RegisterCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IBotTransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly ILogger<RegisterCommandHandler> _logger;


        public RegisterCommandHandler(
            IBotMessagesService botMessagesService,
            IBotTransactionService transactionService,
            IUserService userService,
            ILogger<RegisterCommandHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _transactionService = transactionService;
            _userService = userService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Profile command received from {userId}", userId);

            if (await _userService.CheckIfExists(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "Account for this user is already exists. To edit it you can use /profile_edit command");
                return;
            }

            if (await _transactionService.Get(userId) is { } startedTransaction)
            {
                if (startedTransaction.TransactionType is TransactionType.Registration)
                {
                    await _botMessagesService.SendErrorAsync(userId,
                        "For this account registration process has already started." +
                        " To start a new one you should cancel the previous one with a command /cancel");
                    return;
                }

                await _botMessagesService.SendErrorAsync(userId,
                    "You already have an active transaction. " +
                    "Before you can start a new one you must cancel this one with a command cancel.");
                return;
            }
            await CreateRegistrationTransaction(userId, update);
        }

        private async Task CreateRegistrationTransaction(long userId, Update update)
        {
            var userModel = UserModel.CreateNew(userId, update.Message.Text);

            await _userService.Add(userModel);
            await _botMessagesService.SendTextMessageAsync(userId, "Send me your first name...");

            var transaction = await _transactionService.Create(userId, TransactionType.Registration);
            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.FirstNameReceived.ToString());
            transaction.TransactionData.AddDataPiece(TransactionDataParameter.UserModel, userModel);

            await _transactionService.Add(transaction);
        }
    }
}
