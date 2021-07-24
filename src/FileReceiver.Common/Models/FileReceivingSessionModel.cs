using System;

using FileReceiver.Common.Enums;

namespace FileReceiver.Common.Models
{
    public class FileReceivingSessionModel
    {
        public Guid Id { get; set; }

        public long UserId { get; set; }
        public UserModel User { get; set; }

        public FileReceivingSessionState SessionState { get; set; }
        public int FilesReceived { get; set; }
        public int MaxFiles { get; set; }

        public ConstraintsModel Constrains { get; set; }

        public DateTimeOffset CreateData { get; set; }
        public DateTimeOffset? EndDate { get; set; }

        public SessionEndReason? SessionEndReason { get; set; }

    }
}
