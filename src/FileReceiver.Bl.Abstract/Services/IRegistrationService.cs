using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IRegistrationService
    {
        Task ProcessUserRegistration(Update update);
    }
}
