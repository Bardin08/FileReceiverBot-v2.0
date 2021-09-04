using System;

using FileReceiver.Common.Enums;

namespace FileReceiver.Common.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string TelegramTag { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string SecretWordHash { get; set; }
        public RegistrationState RegistrationState { get; set; }

        public DateTimeOffset RegistrationStartTimestamp { get; set; }
        public DateTimeOffset? RegistrationEndTimestamp { get; set; }

        public static UserModel CreateNew(long userId, string tgTag)
        {
            return new UserModel()
            {
                Id = userId,
                TelegramTag = tgTag,
                RegistrationState = RegistrationState.NewUser,
                RegistrationStartTimestamp = DateTimeOffset.UtcNow,
            };
        }
    }
}
