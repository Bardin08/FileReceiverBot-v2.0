using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class SendFileCommandHandler : ICommandHandler
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public SendFileCommandHandler(
            IBotMessagesService botMessagesService,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _botMessagesService = botMessagesService;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();

            if (!await _userRepository.CheckIfUserExistsAsync(userId))
            {
                await _botMessagesService.SendErrorAsync(userId,
                    "You should register before you can use this command, to do this you can use command /register");
                return;
            }

            var transactionData = new TransactionDataModel();
            transactionData.AddDataPiece(TransactionDataParameter.FileReceivingState,
                FileReceivingState.TokenReceived);

            var transaction = new TransactionModel()
            {
                UserId = userId,
                TransactionType = TransactionType.FileSending,
                TransactionState = TransactionState.Active,
                TransactionData = transactionData,
            };

            await _transactionRepository.AddAsync(_mapper.Map<TransactionEntity>(transaction));
            await _botMessagesService.SendTextMessageAsync(userId, "Okay, send me the session token");
        }
    }
}
