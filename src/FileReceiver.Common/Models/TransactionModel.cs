using System;

using FileReceiver.Common.Enums;

namespace FileReceiver.Common.Models
{
    public class TransactionModel
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }

        public TransactionType TransactionType { get; set; }
        public TransactionDataModel TransactionData { get; set; }
        public TransactionState TransactionState { get; set; }
    }
}
