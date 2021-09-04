using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;

using Microsoft.Extensions.Logging;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class CancelCommandHandler : ICommandHandler
    {
        private readonly IBotTransactionService _transactionService;
        private readonly IBotMessagesService _botMessagesService;
        private readonly ILogger<CancelCommandHandler> _logger;

        public CancelCommandHandler(
            IBotTransactionService transactionService,
            IBotMessagesService botMessagesService,
            ILogger<CancelCommandHandler> logger)
        {
            _transactionService = transactionService;
            _botMessagesService = botMessagesService;
            _logger = logger;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();
            _logger.LogDebug("Cancel command received from {userId}", userId);

            await _transactionService.AbortAllForUser(userId);
            await _botMessagesService.SendMenuAsync(userId);
        }
    }
}
