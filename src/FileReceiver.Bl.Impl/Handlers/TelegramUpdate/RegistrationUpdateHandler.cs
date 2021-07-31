using System;
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
    public class RegistrationUpdateHandler : IUpdateHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRegistrationService _registrationService;
        private readonly ITransactionRepository _transactionRepository;

        public RegistrationUpdateHandler(
            IBotMessagesService botMessagesService,
            IUserRegistrationService registrationService,
            ITransactionRepository transactionRepository)
        {
            _botMessagesService = botMessagesService;
            _registrationService = registrationService;
            _transactionRepository = transactionRepository;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var userId = update.GetTgUserId();

            var transactionEntity = await _transactionRepository
                .GetByUserIdAsync(userId, TransactionTypeDb.Registration);

            var transactionData = new TransactionDataModel(transactionEntity.TransactionData);
            var registrationState = Enum.Parse<RegistrationState>((string)transactionData
                .GetDataPiece(TransactionDataParameter.RegistrationState));

            var transactionProcessModel = new RegistrationProcessingModel()
            {
                UserId = userId,
                Update = update,
                TransactionEntity = transactionEntity,
                TransactionData = transactionData,
            };

            switch (registrationState)
            {
                case RegistrationState.NewUser:
                    break;
                case RegistrationState.FirstNameReceived:
                    await ProcessFirstNameReceivedStateAsync(transactionProcessModel);
                    break;
                case RegistrationState.LastNameReceived:
                    await ProcessLastNameReceivedStateAsync(transactionProcessModel);
                    break;
                case RegistrationState.SecretWordReceived:
                    await ProcessSecretWordReceivedStateAsync(transactionProcessModel);
                    break;
                case RegistrationState.RegistrationComplete:
                    await ProcessRegistrationCompleteStateAsync(transactionProcessModel);
                    break;
                default:
                    await _botMessagesService.SendErrorAsync(userId,
                        "Invalid registration state. Use command /cancel and then click to the register button");
                    break;
            }
        }

        private async Task ProcessFirstNameReceivedStateAsync(RegistrationProcessingModel model)
        {
            var firstName = model.Update.Message.Text;

            await _registrationService.SetFirstNameAsync(model.UserId, firstName);
            await _botMessagesService.SendTextMessageAsync(model.UserId,
                $"Great, {firstName}, now I need your lastname to continue");
        }

        private async Task ProcessLastNameReceivedStateAsync(RegistrationProcessingModel model)
        {
            var lastName = model.Update.Message.Text;

            await _registrationService.SetLastNameAsync(model.UserId, lastName);
            await _botMessagesService.SendTextMessageAsync(model.UserId,
                "Great, and the last step please send me a word or a sentence" +
                " which I'll sometimes to confirm your actions");
        }

        private async Task ProcessSecretWordReceivedStateAsync(RegistrationProcessingModel model)
        {
            var secretWord = model.Update.Message.Text;

            await _registrationService.SetSecretWordAsync(model.UserId, secretWord);
            await ProcessRegistrationCompleteStateAsync(model);
        }

        private async Task ProcessRegistrationCompleteStateAsync(RegistrationProcessingModel model)
        {
            var user = await _registrationService.CompleteRegistrationAsync(model.UserId);

            await _botMessagesService.SendTextMessageAsync(model.UserId, $"Great {user.FirstName}, now you're registered");
            await _botMessagesService.SendMenuAsync(model.UserId);
        }

        private class RegistrationProcessingModel
        {
            public long UserId { get; init; }
            public TransactionEntity TransactionEntity { get; init; }
            public TransactionDataModel TransactionData { get; init; }
            public Update Update { get; init; }
        }
    }
}
