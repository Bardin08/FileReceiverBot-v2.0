using System;
using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public UserRegistrationService(
            IBotMessagesService botMessagesService,
            ITransactionRepository transactionRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _botMessagesService = botMessagesService;
            _userRepository = userRepository;
        }

        public Task CreateNewUserAsync(Update update)
        {
            throw new System.NotImplementedException();
        }

        public async Task SetFirstNameAsync(long userId, string firstName)
        {
            var transactionEntity =
                await _transactionRepository.GetByUserIdAsync(userId, TransactionTypeDb.Registration);
            var transaction = _mapper.Map<TransactionModel>(transactionEntity);

            var userModel = transaction.TransactionData.GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            userModel.FirstName = firstName;
            transactionEntity.User.FirstName = firstName;
            transactionEntity.User.RegistrationState = RegistrationStateDb.LastNameReceived;

            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.UserModel, userModel);
            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.LastNameReceived.ToString());

            transactionEntity.TransactionData = transaction.TransactionData.ParametersAsJson;
            await _transactionRepository.UpdateAsync(transactionEntity);
        }

        public async Task SetLastNameAsync(long userId, string lastName)
        {
            var transactionEntity =
                await _transactionRepository.GetByUserIdAsync(userId, TransactionTypeDb.Registration);
            var transaction = _mapper.Map<TransactionModel>(transactionEntity);

            var userModel = transaction.TransactionData
                .GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            userModel.LastName = lastName;
            transactionEntity.User.LastName = lastName;
            transactionEntity.User.RegistrationState = RegistrationStateDb.SecretWordReceived;

            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.UserModel, userModel);
            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.SecretWordReceived.ToString());

            transactionEntity.TransactionData = transaction.TransactionData.ParametersAsJson;
            await _transactionRepository.UpdateAsync(transactionEntity);
        }

        public async Task SetSecretWordAsync(long userId, string secretWord)
        {
            var transactionEntity =
                await _transactionRepository.GetByUserIdAsync(userId, TransactionTypeDb.Registration);
            var transaction = _mapper.Map<TransactionModel>(transactionEntity);

            var userModel = transaction.TransactionData
                .GetDataPiece<UserModel>(TransactionDataParameter.UserModel);

            userModel.SecretWordHash = secretWord.CreateHash();
            transactionEntity.User.SecretWordHash = secretWord.CreateHash();
            transactionEntity.User.RegistrationState = RegistrationStateDb.RegistrationComplete;

            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.UserModel, userModel);
            transaction.TransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.SecretWordReceived.ToString());

            transactionEntity.TransactionData = transaction.TransactionData.ParametersAsJson;
            await _transactionRepository.UpdateAsync(transactionEntity);
        }

        public async Task<UserModel> CompleteRegistrationAsync(long userId)
        {
            var registrationTransactionEntity = await _transactionRepository
                .GetByUserIdAsync(userId, TransactionTypeDb.Registration);

            if (registrationTransactionEntity is null)
            {
                return null;
            }

            var transaction = _mapper.Map<TransactionModel>(registrationTransactionEntity);

            var userModel = transaction.TransactionData.GetDataPiece<UserModel>(TransactionDataParameter.UserModel);
            userModel.RegistrationState = RegistrationState.RegistrationComplete;
            transaction.TransactionData.UpdateParameter(TransactionDataParameter.UserModel, userModel);
            transaction.TransactionData.UpdateParameter(TransactionDataParameter.RegistrationState,
                RegistrationState.RegistrationComplete);

            registrationTransactionEntity.TransactionData = transaction.TransactionData.ParametersAsJson;
            registrationTransactionEntity.TransactionState = TransactionStateDb.Committed;

            await UpdateUserEntityAsync(userModel);
            await _transactionRepository.UpdateAsync(registrationTransactionEntity);

            return userModel;
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
                existingUser.RegistrationEndTimestamp = DateTimeOffset.UtcNow;

                await _userRepository.UpdateAsync(existingUser);
            }
        }
    }
}
