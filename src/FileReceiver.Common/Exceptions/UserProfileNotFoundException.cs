using System;

namespace FileReceiver.Common.Exceptions
{
    public class UserProfileNotFoundException : Exception
    {
        public long TelegramUserId { get; set; }
        public string DesiredAction { get; set; }

        public UserProfileNotFoundException(long tgUserId, string desiredAction)
        {
            TelegramUserId = tgUserId;
            DesiredAction = desiredAction;
        }
    }
}
