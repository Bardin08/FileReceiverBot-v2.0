using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Services
{
    public class UpdateHandlerService : IUpdateHandlerService
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly ICommandHandlerFactory _commandHandlerFactory;
        private readonly IUpdateHandlerFactory _updateHandlerFactory;
        private readonly ICallbackQueryHandlerFactory _callbackQueryFactory;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public UpdateHandlerService(
            IBotMessagesService botMessagesService,
            ICommandHandlerFactory commandHandlerFactory,
            IUpdateHandlerFactory updateHandlerFactory,
            ICallbackQueryHandlerFactory callbackQueryFactory,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _botMessagesService = botMessagesService;
            _commandHandlerFactory = commandHandlerFactory;
            _updateHandlerFactory = updateHandlerFactory;
            _callbackQueryFactory = callbackQueryFactory;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            switch (update)
            {
                case { Message: { Text: { } } }:
                    await HandleMessageAsync(update);
                    break;
                case { Message: { Document: { } } or { Photo: { } } }:
                    await HandleMessageAsync(update);
                    break;
                case { CallbackQuery: { Message: { Text: { } } } cb }:
                    await HandleCallbackQuery(cb);
                    break;
            }
        }

        private async Task HandleMessageAsync(Update update)
        {
            var userId = update.GetTgUserId();
            var lastActiveTransactionForUser =
                _mapper.Map<TransactionModel>(await _transactionRepository.GetLastActiveTransactionByUserId(userId));
            if (lastActiveTransactionForUser == null || update.Message.Text is "/cancel")
            {
                if (await TryGetCommandHandlerAsync(update.Message) is { } commandHandler)
                {
                    await commandHandler.HandleCommandAsync(update);
                    return;
                }
            }

            if (update.Message.Text.IsPossibleCommand())
            {
                await _botMessagesService.SendErrorAsync(userId, "You have uncompleted transactions." +
                                                                 " To abort them use commend /cancel");
                return;
            }

            if (lastActiveTransactionForUser != null &&
                (await TryGetUpdateHandlerAsync(lastActiveTransactionForUser)) is { } updateHandler)
            {
                await updateHandler.HandleUpdateAsync(update);
            }
        }

        private async Task HandleCallbackQuery(CallbackQuery cb)
        {
            switch (cb)
            {
                case { Message: { Text: { } } }:
                    await _callbackQueryFactory.CreateCallbackHandler(cb).HandleCallback(cb);
                    break;
            }
        }

        private Task<IUpdateHandler> TryGetUpdateHandlerAsync(TransactionModel transaction)
        {
            return Task.FromResult(_updateHandlerFactory.CreateUpdateHandler(transaction.TransactionType));
        }

        private Task<ICommandHandler> TryGetCommandHandlerAsync(Message message)
        {
            if (message.Text.IsPossibleCommand())
            {
                return Task.FromResult(
                    _commandHandlerFactory.CreateCommandHandler(message.Text.GetCommandFromMessage()));
            }

            return Task.FromResult<ICommandHandler>(null);
        }
    }
}
