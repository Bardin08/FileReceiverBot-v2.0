﻿using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class CancelCommandHandler : ICommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBotMessagesService _botMessagesService;

        public CancelCommandHandler(
            IUserRepository userRepository,
            ITransactionRepository transactionRepository, IBotMessagesService botMessagesService)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _botMessagesService = botMessagesService;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.Message.From.Id;

            var activeTransaction = await _transactionRepository.GetLastActiveTransactionByUserId(userId);
            if (activeTransaction != null)
            {
                activeTransaction.TransactionState = TransactionStateDb.Aborted;
                await _transactionRepository.UpdateAsync(activeTransaction);
            }

            await _botMessagesService.SendMenuAsync(userId);
        }
    }
}
