using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Abstract.Repositories;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.Commands
{
    public class CancelCommandHandler : ICommandHandler
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBotMessagesService _botMessagesService;

        public CancelCommandHandler(
            ITransactionRepository transactionRepository,
            IBotMessagesService botMessagesService)
        {
            _transactionRepository = transactionRepository;
            _botMessagesService = botMessagesService;
        }

        public async Task HandleCommandAsync(Update update)
        {
            var userId = update.GetTgUserId();

            await _transactionRepository.AbortAllTransactionsForUser(userId);

            await _botMessagesService.SendMenuAsync(userId);
        }
    }
}
