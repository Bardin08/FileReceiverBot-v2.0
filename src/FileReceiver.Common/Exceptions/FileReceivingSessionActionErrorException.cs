using System;

namespace FileReceiver.Common.Exceptions
{
    public sealed class FileReceivingSessionActionErrorException : Exception
    {
        public long TelegramUserId { get; }
        public string SessionAction { get; }
        public new string Message { get; private set; }

#nullable enable
        public FileReceivingSessionActionErrorException(
            long tgUserId,
            string sessionAction,
            Exception? innerException) : base(String.Empty, innerException)
        {
            TelegramUserId = tgUserId;
            SessionAction = sessionAction;

            SetUnderstandableMessage();
        }
#nullable restore

        private void SetUnderstandableMessage()
        {
            Message = SessionAction switch
            {
                "SetFilesStorageAsync" => "An error occured while setting files amount constraint.",
                "ExecuteSessionAsync" => "An error occured while executing the transaction.",
                "StopSessionAsync" => "An error occured while stopping the transaction.",
                "GetId" => "An error occured while receiving session's Id.",
                _ => "An error occured.",
            };
        }
    }
}
