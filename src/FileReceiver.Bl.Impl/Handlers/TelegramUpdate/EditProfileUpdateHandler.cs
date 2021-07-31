using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.TelegramUpdate
{
    public class EditProfileUpdateHandler : IUpdateHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;

        public EditProfileUpdateHandler(
            IBotMessagesService botMessagesService,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository)
        {
            _botMessagesService = botMessagesService;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var userId = update.GetTgUserId();

            if (!await _transactionRepository.CheckIfTransactionForUserExists(
                userId, TransactionTypeDb.EditProfile, TransactionStateDb.Active))
            {
                await _botMessagesService.SendErrorAsync(userId, $"Error token: {Guid.NewGuid().ToString().Substring(0, 8)}");
                return;
            }

            var transactionEntity = await _transactionRepository
                .GetByUserIdAsync(userId, TransactionTypeDb.EditProfile);

            var transactionData = new TransactionDataModel(transactionEntity.TransactionData);
            var editingAction = Enum.Parse<ProfileEditingAction>((string)transactionData
                .GetDataPiece(TransactionDataParameter.ProfileEditingAction));

            var transactionProcessModel = new ProfileEditingDataDto()
            {
                UserId = userId,
                Update = update,
                TransactionEntity = transactionEntity,
                TransactionData = transactionData,
            };

            // TODO: rewrite with pattern matching
            switch (editingAction)
            {
                case ProfileEditingAction.UpdateFirstName:
                    await HandleFirstNameUpdate(transactionProcessModel);
                    break;
                case ProfileEditingAction.UpdateLastName:
                    await HandleLastNameUpdate(transactionProcessModel);
                    break;
                case ProfileEditingAction.UpdateSecretWord:
                    await HandleSecretWordUpdate(transactionProcessModel);
                    break;
                default:
                    await _botMessagesService.SendErrorAsync(userId,
                        "Invalid profile editing action. Use command /cancel and then use command /profile_edit");
                    break;
            }
        }

        private async Task HandleFirstNameUpdate(ProfileEditingDataDto model)
        {
            var userEntity = await _userRepository.GetByIdAsync(model.UserId);
            var newFirstName = model.Update.Message.Text;

            userEntity.FirstName = newFirstName;
            await _userRepository.UpdateAsync(userEntity);

            model.TransactionEntity.TransactionState = TransactionStateDb.Committed;
            await _transactionRepository.UpdateAsync(model.TransactionEntity);

            await _botMessagesService.SendTextMessageAsync(model.UserId,
                "Great! Your first name was successfully updated!");
        }

        private async Task HandleLastNameUpdate(ProfileEditingDataDto model)
        {
            var userEntity = await _userRepository.GetByIdAsync(model.UserId);
            var newLastName = model.Update.Message.Text;

            userEntity.LastName = newLastName;
            await _userRepository.UpdateAsync(userEntity);

            model.TransactionEntity.TransactionState = TransactionStateDb.Committed;
            await _transactionRepository.UpdateAsync(model.TransactionEntity);

            await _botMessagesService.SendTextMessageAsync(model.UserId,
                "Great! Your last name was successfully updated!");
        }

        private async Task HandleSecretWordUpdate(ProfileEditingDataDto model)
        {
            var userEntity = await _userRepository.GetByIdAsync(model.UserId);
            var newSecretWord = model.Update.Message.Text;

            userEntity.SecretWordHash = newSecretWord.CreateHash();
            await _userRepository.UpdateAsync(userEntity);

            model.TransactionEntity.TransactionState = TransactionStateDb.Committed;
            await _transactionRepository.UpdateAsync(model.TransactionEntity);

            await _botMessagesService.SendTextMessageAsync(model.UserId,
                "Great! Your secret word was successfully updated!");
        }

        private sealed class ProfileEditingDataDto
        {
            public long UserId { get; set; }
            public Update Update { get; set; }
            public TransactionEntity TransactionEntity { get; set; }
            public TransactionDataModel TransactionData { get; set; }
        }
    }
}
