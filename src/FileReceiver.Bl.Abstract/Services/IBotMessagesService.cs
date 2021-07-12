using System.Threading.Tasks;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IBotMessagesService
    {
        Task SendMenuAsync(long userOrChatId);
        Task SendErrorAsync(long userOrChatId, string errorMessage);
        Task SendTextMessageAsync(long userOrChatId, string message);
        Task SendNotSupportedAsync(long userOrChatId, string notSupportedActionName);
    }
}
