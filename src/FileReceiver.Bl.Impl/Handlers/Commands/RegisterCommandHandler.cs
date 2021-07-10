using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

using Microsoft.Extensions.Internal;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class RegisterCommandHandler : ICommandHandler
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBotMessagesService _botMessagesService;
        private readonly ISystemClock _systemClock;
        private readonly IMapper _mapper;

        public RegisterCommandHandler(
            IBotMessagesService botMessagesService,
            ITransactionRepository transactionRepository,
            IUserRepository userRepository,
            ISystemClock systemClock,
            IMapper mapper)
        {
            _botMessagesService = botMessagesService;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _systemClock = systemClock;
            _mapper = mapper;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.Message.From.Id;

            if (await TryGetUserProfile(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "Account for this account registration process has already started." +
                    " To start a new one you should cancel the previous one with a command /cancel");
                return;
            }
            
            if (await TryGetRegistrationTransaction(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "Account for this user is already exists. To edit it you can use /profile_update command");
                return;
            }

            var registrationTransactionData = new TransactionDataModel();

            var userModel = new UserModel()
            {
                Id = userId,
                TelegramTag = update.Message.From.Username,
                RegistrationState = RegistrationState.NewUser,
                RegistrationStartTimestamp = _systemClock.UtcNow,
            };

            await _userRepository.AddAsync(_mapper.Map<UserEntity>(userModel));

            registrationTransactionData.AddDataPiece(TransactionDataParameter.UserModel, userModel);
            var registrationTransaction = new TransactionModel()
            {
                UserId = userId,
                TransactionType = TransactionType.Registration,
                TransactionState = TransactionState.Active,
                TransactionDataModel = new TransactionDataModel()
            };
            await _transactionRepository.AddAsync(_mapper.Map<TransactionEntity>(registrationTransaction));
        }

        private async Task<bool> TryGetRegistrationTransaction(long userId)
        {
            return (await _transactionRepository.GetByUserIdAsync(userId, TransactionTypeDb.Registration)) != null;
        }

        private async Task<bool> TryGetUserProfile(long userId)
        {
            return (await _userRepository.GetByIdAsync(userId)) != null;
        }
    }
}
