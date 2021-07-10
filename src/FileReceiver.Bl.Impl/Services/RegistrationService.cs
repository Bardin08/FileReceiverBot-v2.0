using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Dal.Abstract.Repositories;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepository;

        public RegistrationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ProcessUserRegistration(Update update)
        {
            var userId = update.Message.From.Id;
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                await CreateNewUserAsync(userId);
                return;
            }

            await NextRegistrationStepAsync();
        }

        private async Task NextRegistrationStepAsync()
        {
            throw new System.NotImplementedException();
        }

        private async Task CreateNewUserAsync(long userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
