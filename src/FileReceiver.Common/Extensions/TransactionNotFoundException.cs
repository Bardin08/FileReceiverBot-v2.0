using System;

using FileReceiver.Common.Enums;

namespace FileReceiver.Common.Extensions
{
    public class TransactionNotFoundException : Exception
    {
        public long UserId { get; }
        public TransactionType TransactionType { get; }

        public TransactionNotFoundException(long userId, TransactionType transactionType)
            : base($"Transaction with type {transactionType} wasn't found for user {userId}")
        {
            UserId = userId;
            TransactionType = transactionType;
        }
    }
}
