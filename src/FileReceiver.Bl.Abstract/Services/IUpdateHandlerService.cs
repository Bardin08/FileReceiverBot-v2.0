using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IUpdateHandlerService
    {
        Task HandleUpdateAsync(Update update);
    }
}
