using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Handlers
{
    public interface ICommandHandler
    {
        Task HandleCommandAsync(Update update);
    }
}
