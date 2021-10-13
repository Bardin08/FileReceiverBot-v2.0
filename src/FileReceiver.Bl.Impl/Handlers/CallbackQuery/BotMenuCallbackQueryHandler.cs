using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Handlers.CallbackQuery
{
    public class BotMenuCallbackQueryHandler : ICallbackQueryHandler
    {
        private readonly ICommandHandlerFactory _commandHandlerFactory;

        public BotMenuCallbackQueryHandler(ICommandHandlerFactory commandHandlerFactory)
        {
            _commandHandlerFactory = commandHandlerFactory;
        }

        public async Task HandleCallback(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {
            var update = new Update()
            {
                CallbackQuery = callbackQuery
            };

            switch (callbackQuery)
            {
                case { Data: "menu-/profile", From: { } from }:
                    await _commandHandlerFactory
                        .CreateCommandHandler("/profile")
                        .HandleCommandAsync(update);
                    break;
                case { Data: "menu-/send_file", From: { } from }:
                    await _commandHandlerFactory
                        .CreateCommandHandler("/send_file")
                        .HandleCommandAsync(update);
                    break;
            }
        }
    }
}
