using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;

using Microsoft.Extensions.Logging;

namespace FileReceiver.Bl.Impl.Handlers.CallbackQuery
{
    public class EditProfileCallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserService _userService;
        private readonly IBotTransactionService _transactionService;
        private readonly ILogger<EditProfileCallbackQueryHandler> _logger;

        public EditProfileCallbackQueryHandler(
            IBotMessagesService botMessagesService,
            IUserService userService,
            IBotTransactionService transactionService,
            ILogger<EditProfileCallbackQueryHandler> logger)
        {
            _botMessagesService = botMessagesService;
            _userService = userService;
            _transactionService = transactionService;
            _logger = logger;
        }

        public async Task HandleCallback(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            switch (callbackQuery)
            {
                case { Data: "profile-edit-first-name", From: { } from }:
                    await CreateProfileEditingTransaction(from.Id, ProfileEditingAction.UpdateFirstName,
                        "Okay, let's change your first name. Send me the new one plz");
                    break;
                case { Data: "profile-edit-last-name", From: { } from }:
                    await CreateProfileEditingTransaction(from.Id, ProfileEditingAction.UpdateLastName,
                        "Okay, let's change your last name. Send me the new one plz");
                    break;
                case { Data: "profile-edit-secret-word", From: { } from }:
                    await CreateProfileEditingTransaction(from.Id, ProfileEditingAction.UpdateSecretWord,
                        "Okay, let's change your secret word. Send me the new one plz");
                    break;
            }
        }

        private async Task CreateProfileEditingTransaction(long userId, ProfileEditingAction editingAction,
            string messageForUser)
        {
            _logger.LogDebug("Edit profile callback query with edit action {editAction} received from {userId}.",
                editingAction, userId);
            await _userService.CompleteRegistrationAsync(userId);
            var userLastTransaction = await _transactionService.GetNullIfNotExists(userId);
            if (userLastTransaction is not null)
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "You have uncompleted action. Use command /cancel to undo it and that use a command /profile_edit");
                _logger.LogWarning("Profile editing transaction wasn't created for {userId} " +
                                   "because he has an uncompleted transaction with {activeTransactionId}",
                    userId, userLastTransaction.Id);
                return;
            }

            var transaction = await _transactionService.Create(userId, TransactionType.EditProfile);
            transaction.TransactionData.AddDataPiece(TransactionDataParameter.ProfileEditingAction, editingAction);
            _logger.LogDebug("Profile editing transaction was created for {userId} " +
                               "transaction id is {activeTransactionId}", userId, transaction.Id);

            await _transactionService.Add(transaction);
            await _botMessagesService.SendTextMessageAsync(userId, messageForUser);
        }
    }
}
