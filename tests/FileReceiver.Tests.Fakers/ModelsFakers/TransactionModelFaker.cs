using System;

using Bogus;

using FileReceiver.Common.Enums;
using FileReceiver.Common.Models;
using FileReceiver.Tests.Fakers.Configurations;
using FileReceiver.Tests.Fakers.EntityFakers;

namespace FileReceiver.Tests.Fakers.ModelsFakers
{
    public class TransactionModelFaker
    {
        internal static Faker<TransactionModel> GetTransactionEntityFaker(int seed = FakersConstants.FakersSeed)
        {
            var user = UserEntityFaker.GetUserEntityFaker(seed).Generate();
            return new Faker<TransactionModel>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, Guid.NewGuid())
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.TransactionType, x => x.PickRandom<TransactionType>())
                .RuleFor(x => x.TransactionData, new TransactionDataModel())
                .RuleFor(x => x.TransactionState, x => x.PickRandom<TransactionState>());
        }

        public static TransactionModel GenerateNewFileSessionTransactionModel(long userId,
            int seed = FakersConstants.FakersSeed)
        {
            return GetTransactionEntityFaker(seed)
                .RuleFor(x => x.UserId, userId)
                .RuleFor(x => x.TransactionState, TransactionState.Active)
                .RuleFor(x => x.TransactionType, TransactionType.FileReceivingSessionCreating);
        }

        public static TransactionModel GenerateTransactionEntityWithFileReceivingSessionId(long userId,
            Guid sessionId, int seed = FakersConstants.FakersSeed)
        {
            var transactionData = new TransactionDataModel();
            transactionData.AddDataPiece(TransactionDataParameter.FileReceivingSessionId, sessionId.ToString());

            return GetTransactionEntityFaker(seed)
                .RuleFor(x => x.UserId, userId)
                .RuleFor(x => x.TransactionState, TransactionState.Active)
                .RuleFor(x => x.TransactionType, TransactionType.FileReceivingSessionCreating)
                .RuleFor(x => x.TransactionData, transactionData);
        }
    }
}
