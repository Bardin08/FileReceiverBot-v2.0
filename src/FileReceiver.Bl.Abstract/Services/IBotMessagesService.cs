using System.Threading.Tasks;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IBotMessagesService
    {
        Task SendMenuAsync(long userOrChatId);
        Task SendErrorAsync(long userOrChatId, string errorMessage);
        Task SendNotSupportedAsync(long userOrChatId, string notSupportedActionName);
    }
}
