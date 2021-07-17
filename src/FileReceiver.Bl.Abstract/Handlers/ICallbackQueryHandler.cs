using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Handlers
{
    public interface ICallbackQueryHandler
    {
        public Task HandleCallback(CallbackQuery callbackQuery);
    }
}
