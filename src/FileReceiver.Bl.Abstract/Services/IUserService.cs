using System.Threading.Tasks;

using FileReceiver.Common.Models;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IUserService
    {
        Task SetFirstNameAsync(long userId, string firstName);
        Task SetLastNameAsync(long userId, string lastName);
        Task SetSecretWordAsync(long userId, string secretWord);
        Task<UserModel> CompleteRegistrationAsync(long userId);
        Task<bool> CheckIfExists(long userId);
        Task<UserModel> Get(long userId);
        Task<UserModel> Add(UserModel user);
    }
}
