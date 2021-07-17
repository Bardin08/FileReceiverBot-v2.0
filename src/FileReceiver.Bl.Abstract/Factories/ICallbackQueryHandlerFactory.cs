using FileReceiver.Bl.Abstract.Handlers;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Factories
{
    public interface ICallbackQueryHandlerFactory
    {
        ICallbackQueryHandler CreateCallbackHandler(CallbackQuery callbackQuery);
    }
}
