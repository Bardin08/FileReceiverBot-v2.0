using System.Threading.Tasks;

using Telegram.Bot.Types.ReplyMarkups;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IBotMessagesService
    {
        Task SendMenuAsync(long userOrChatId);
        Task SendErrorAsync(long userOrChatId, string errorMessage);
        Task SendTextMessageAsync(long userOrChatId, string message);
        Task SendMessageWithKeyboardAsync(long userOrChatId, string message, IReplyMarkup keyboard);
        Task SendNotSupportedAsync(long userOrChatId, string notSupportedActionName);
    }
}
