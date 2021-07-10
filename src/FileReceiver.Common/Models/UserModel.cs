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
    }
}
