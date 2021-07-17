using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.CallbackQuery
{
    public class EditProfileCallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IUpdateHandlerFactory _updateHandlerFactory;

        public EditProfileCallbackQueryHandler(
            IBotMessagesService botMessagesService,
            IUserRegistrationService userRegistrationService,
            ITransactionRepository transactionRepository,
            IMapper mapper, IUpdateHandlerFactory updateHandlerFactory)
        {
            _botMessagesService = botMessagesService;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _updateHandlerFactory = updateHandlerFactory;
            _userRegistrationService = userRegistrationService;
        }

        public async Task HandleCallback(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            switch (callbackQuery)
            {
                case { Data: "profile-edit-first-name", From: { } from }:
                    await CreateProfileEditingTransactionAsync(from.Id, ProfileEditingAction.UpdateFirstName,
                        "Okay, let's change your first name. Send me the new one plz");
                    break;
                case { Data: "profile-edit-last-name", From: { } from }:
                    await CreateProfileEditingTransactionAsync(from.Id, ProfileEditingAction.UpdateLastName,
                        "Okay, let's change your last name. Send me the new one plz");
                    break;
                case { Data: "profile-edit-secret-word", From: { } from }:
                    await CreateProfileEditingTransactionAsync(from.Id, ProfileEditingAction.UpdateSecretWord,
                        "Okay, let's change your secret word. Send me the new one plz");
                    break;
            }
        }

        private async Task CreateProfileEditingTransactionAsync(long fromId, ProfileEditingAction editingAction,
            string messageForUser)
        {
            await _userRegistrationService.CompleteRegistrationAsync(fromId);
            if (await CheckIfUserHasUncompletedTransactionAndNotify(fromId))
            {
                return;
            }

            var transactionData = new TransactionDataModel();
            transactionData.AddDataPiece(TransactionDataParameter.ProfileEditingAction,
                editingAction);

            var transaction = new TransactionModel()
            {
                UserId = fromId,
                TransactionType = TransactionType.EditProfile,
                TransactionState = TransactionState.Active,
                TransactionData = transactionData,
            };
            await _transactionRepository.AddAsync(_mapper.Map<TransactionEntity>(transaction));
            await _botMessagesService.SendTextMessageAsync(fromId, messageForUser);
        }

        private async Task<bool> CheckIfUserHasUncompletedTransactionAndNotify(long fromId)
        {
            if (await _transactionRepository.GetLastActiveTransactionByUserId(fromId) is null)
            {
                return false;
            }

            await _botMessagesService.SendErrorAsync(fromId,
                "You have uncompleted action. Use command /cancel to undo it and that use a command /profile_edit");
            return true;
        }
    }
}
