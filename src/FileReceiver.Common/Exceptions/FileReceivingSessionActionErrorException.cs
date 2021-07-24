using System;

namespace FileReceiver.Common.Exceptions
{
    public sealed class FileReceivingSessionActionErrorException : Exception
    {
        public long TelegramUserId { get; set; }
        public string SessionAction { get; set; }
        public new string Message { get; private set; }

        public FileReceivingSessionActionErrorException(long tgUserId, string sessionAction)
        {
            TelegramUserId = tgUserId;
            SessionAction = sessionAction;

            SetUnderstandableMessage();
        }

        private void SetUnderstandableMessage()
        {
            Message = SessionAction switch
            {
                "SetFilesStorageAsync" => "An error occured while setting files amount constraint.",
                "ExecuteSessionAsync" => "An error occured while executing the transaction.",
                "StopSessionAsync" => "An error occured while stopping the transaction.",
                _ => "An error occured.",
            };
        }
    }
}
