#nullable enable
using System;

namespace FileReceiver.Common.Exceptions
{
    public class FileReceivingSessionNotFound : Exception
    {
        public Guid DesiredSessionId { get; private set; }

        public FileReceivingSessionNotFound(Guid desiredSessionId, string? message = "", Exception? innerException = null)
            : base(message, innerException)
        {
            DesiredSessionId = desiredSessionId;
        }
    }
}
#nullable restore
