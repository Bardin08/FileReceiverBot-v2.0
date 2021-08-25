using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Abstract.Repositories;

using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class CancelCommandHandler : ICommandHandler
    {
        private readonly IBotTransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBotMessagesService _botMessagesService;
        private readonly ILogger<CancelCommandHandler> _logger;

        public CancelCommandHandler(
            IBotTransactionService transactionService,
            ITransactionRepository transactionRepository,
            IBotMessagesService botMessagesService,
            ILogger<CancelCommandHandler> logger)
        {
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
            _botMessagesService = botMessagesService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Cancel command received from {userId}", userId);

            await _transactionRepository.AbortAllTransactionsForUser(userId);

            await _botMessagesService.SendMenuAsync(userId);
        }
    }
}
