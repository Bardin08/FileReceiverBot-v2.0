using System;
using System.IO;

namespace FileReceiver.Integrations.Mega.Models
{
    public record MegaActionResponse
    {
        private MegaActionResponse()
        {
        }

        public Guid TransactionId { get; private init; }
        public string Token { get; init; }
        public bool Successful { get; init; }
        public ActionDetails ActionDetails { get; private init; }

        public static MegaActionResponse Success(Guid transactionId, string token, ActionDetails details = null)
        {
            return new MegaActionResponse()
            {
                Successful = true,
                TransactionId = transactionId,
                Token = token,
                ActionDetails = details,
            };
        }

        public static MegaActionResponse Fail(Guid transactionId, string token, ActionDetails details = null)
        {
            return new MegaActionResponse()
            {
                Successful = false,
                TransactionId = transactionId,
                Token = token,
                ActionDetails = details,
            };
        }
    }

    public record ActionDetails
    {
        public Guid ActionId { get; init; }
        public MegaActionType ActionType { get; init; }
        public DateTimeOffset ActionTimestamp { get; init; }
        public string NodeLink { get; set; }
        public Stream Data { get; set; }
    }

    public enum MegaActionType
    {
        CreateFolder,
        DeleteFolder,
        UploadFile,
        DownloadFolder,
    }
}
