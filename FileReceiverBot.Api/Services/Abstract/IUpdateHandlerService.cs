using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace FileReceiverBot.Api.Services.Abstract
{
    public interface IUpdateHandlerService
    {
        Task HandleUpdateAsync(Update update);
    }
}