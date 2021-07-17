using System.Threading.Tasks;

using FileReceiver.Common.Models;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IUserRegistrationService
    {
        Task CreateNewUserAsync(Update update);
        Task SetFirstNameAsync(long userId, string firstName);
        Task SetLastNameAsync(long userId, string lastName);
        Task SetSecretWordAsync(long userId, string secretWord);
        Task<UserModel> CompleteRegistrationAsync(long userId);
    }
}
