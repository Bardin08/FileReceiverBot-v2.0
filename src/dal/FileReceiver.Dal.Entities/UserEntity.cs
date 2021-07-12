using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using FileReceiver.Dal.Entities.Enums;

namespace FileReceiver.Dal.Entities
{
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string TelegramTag { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string SecretWordHash { get; set; }
        public RegistrationStateDb RegistrationState { get; set; }

        public DateTimeOffset RegistrationStartTimestamp { get; set; }
        public DateTimeOffset? RegistrationEndTimestamp { get; set; }
    }
}
