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

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class RegisterCommandHandler : ICommandHandler
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBotMessagesService _botMessagesService;
        private readonly IMapper _mapper;

        public RegisterCommandHandler(
            IBotMessagesService botMessagesService,
            ITransactionRepository transactionRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _botMessagesService = botMessagesService;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();

            if (await _userRepository.CheckIfUserExists(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "Account for this user is already exists. To edit it you can use /profile_edit command");
                return;
            }

            if (await TryGetCompletedRegistrationTransactionAsync(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "For this account registration process has already started." +
                    " To start a new one you should cancel the previous one with a command /cancel");
                return;
            }
            await CreateRegistrationTransaction(userId, update);
        }

        private async Task CreateRegistrationTransaction(long userId, Update update)
        {
            var userModel = new UserModel()
            {
                Id = userId,
                TelegramTag = update.Message.From.Username,
                RegistrationState = RegistrationState.NewUser,
                RegistrationStartTimestamp = DateTimeOffset.UtcNow,
            };

            await _userRepository.AddAsync(_mapper.Map<UserEntity>(userModel));

            var registrationTransactionData = new TransactionDataModel();

            await _botMessagesService.SendTextMessageAsync(userId, "Send me your first name...");

            registrationTransactionData.UpdateParameter(
                TransactionDataParameter.RegistrationState, RegistrationState.FirstNameReceived.ToString());
            registrationTransactionData.AddDataPiece(TransactionDataParameter.UserModel, userModel);

            var registrationTransaction = new TransactionModel()
            {
                UserId = userId,
                TransactionType = TransactionType.Registration,
                TransactionState = TransactionState.Active,
                TransactionData = registrationTransactionData,
            };

            await _transactionRepository.AddAsync(_mapper.Map<TransactionEntity>(registrationTransaction));
        }

        private async Task<bool> TryGetCompletedRegistrationTransactionAsync(long userId)
        {
            return (await _transactionRepository
                .GetCompletedTransactionByUserIdAsync(userId, TransactionTypeDb.Registration)) != null;
        }
    }
}
