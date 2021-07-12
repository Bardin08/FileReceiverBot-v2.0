using System;
using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Microsoft.Extensions.Internal;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.TelegramUpdate
{
    public class RegistrationUpdateHandler : IUpdateHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ISystemClock _systemClock;

        public RegistrationUpdateHandler(IBotMessagesService botMessagesService,
            ITransactionRepository transactionRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ISystemClock systemClock)
        {
            _botMessagesService = botMessagesService;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _systemClock = systemClock;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var userId = update.Message.From.Id;
            
            if (!await _transactionRepository.CheckIfTransactionForUserExists(
                userId, TransactionTypeDb.Registration, TransactionStateDb.Active))
            {
            }

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

            switch(registrationState)
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
            var userModel = model.TransactionData
                .GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            userModel.FirstName = model.Update.Message.Text;
            
            await _botMessagesService.SendTextMessageAsync(model.UserId,
                $"Great, {model.Update.Message.Text}, now I need your lastname to continue");
            model.TransactionData.UpdateParameter(
                TransactionDataParameter.UserModel, userModel);
            model.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.LastNameReceived.ToString());
            
            model.TransactionEntity.TransactionData = model.TransactionData.ParametersAsJson;
            await _transactionRepository.UpdateAsync(model.TransactionEntity);
        }
        
        private async Task ProcessLastNameReceivedStateAsync(RegistrationProcessingModel model)
        {
            var userModel = model.TransactionData
                .GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            userModel.LastName = model.Update.Message.Text;
            
            await _botMessagesService.SendTextMessageAsync(model.UserId,
                "Great, and the last step please send me a word or a sentence" +
                " which I'll sometimes to confirm your actions");
            model.TransactionData.UpdateParameter(
                TransactionDataParameter.UserModel, userModel);
            model.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.SecretWordReceived.ToString());

            model.TransactionEntity.TransactionData = model.TransactionData.ParametersAsJson;
            await _transactionRepository.UpdateAsync(model.TransactionEntity);
        }

        private async Task ProcessSecretWordReceivedStateAsync(RegistrationProcessingModel model)
        {
            var userModel = model.TransactionData
                .GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            userModel.SecretWordHash = model.Update.Message.Text.CreateHash();
            
            await _botMessagesService.SendTextMessageAsync(model.UserId,
                "Great, and the last step please send me a word or a sentence" +
                " which I'll sometimes to confirm your actions");
            model.TransactionData.UpdateParameter(
                TransactionDataParameter.UserModel, userModel);
            model.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.RegistrationComplete);
            model.TransactionEntity.TransactionState = TransactionStateDb.Committed;
            model.TransactionEntity.TransactionData = model.TransactionData.ParametersAsJson;
            await UpdateUserEntityAsync(userModel);

            // TODO: Add confirmation message where user can agree with entered data or enter them all again
            await _transactionRepository.UpdateAsync(model.TransactionEntity);

            await ProcessRegistrationCompleteStateAsync(model);
        }

        private async Task ProcessRegistrationCompleteStateAsync(RegistrationProcessingModel model)
        {
            var userModel = model.TransactionData
                .GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            await _botMessagesService.SendTextMessageAsync(model.UserId,
                $"Great, {userModel.FirstName}. Now you're registered");
            await _botMessagesService.SendMenuAsync(model.UserId);
        }

        private async Task UpdateUserEntityAsync(UserModel userModel)
        {
            var existingUser = await _userRepository.GetByIdAsync(userModel.Id);
            if (existingUser != null)
            {
                var updatedUser = _mapper.Map<UserEntity>(userModel);

                existingUser.FirstName = updatedUser.FirstName;
                existingUser.LastName = updatedUser.LastName;
                existingUser.RegistrationState = updatedUser.RegistrationState;
                existingUser.SecretWordHash = updatedUser.SecretWordHash;
                existingUser.RegistrationEndTimestamp = _systemClock.UtcNow;

                await _userRepository.UpdateAsync(existingUser);
            }
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
