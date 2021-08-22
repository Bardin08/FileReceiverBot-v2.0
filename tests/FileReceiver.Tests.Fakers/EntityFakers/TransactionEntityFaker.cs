using System;

using Bogus;

using FileReceiver.Common.Enums;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;
using FileReceiver.Tests.Fakers.Configurations;

namespace FileReceiver.Tests.Fakers.EntityFakers
{
    public static class TransactionEntityFaker
    {
        internal static Faker<TransactionEntity> GetTransactionEntityFaker(int seed = FakersConstants.FakersSeed)
        {
            var user = UserEntityFaker.GetUserEntityFaker(seed).Generate();
            return new Faker<TransactionEntity>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, Guid.NewGuid())
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.User, user)
                .RuleFor(x => x.TransactionType, x => x.PickRandom<TransactionTypeDb>())
                .RuleFor(x => x.TransactionData, new TransactionDataModel().ParametersAsJson)
                .RuleFor(x => x.TransactionState, x => x.PickRandom<TransactionStateDb>());
        }

        public static TransactionEntity GenerateNewFileSessionTransactionEntity(UserEntity user,
            int seed = FakersConstants.FakersSeed)
        {
            return GetTransactionEntityFaker(seed)
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.User, user)
                .RuleFor(x => x.TransactionState, TransactionStateDb.Active)
                .RuleFor(x => x.TransactionType, TransactionTypeDb.FileReceivingSessionCreating);
        }

        public static TransactionEntity GenerateTransactionEntityWithFileReceivingSessionId(UserEntity user,
            FileReceivingSessionEntity sessionEnt, int seed = FakersConstants.FakersSeed)
        {
            var transactionData = new TransactionDataModel();
            transactionData.AddDataPiece(TransactionDataParameter.FileReceivingSessionId, sessionEnt.Id);

            return GetTransactionEntityFaker(seed)
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.User, user)
                .RuleFor(x => x.TransactionState, TransactionStateDb.Active)
                .RuleFor(x => x.TransactionType, TransactionTypeDb.FileReceivingSessionCreating)
                .RuleFor(x => x.TransactionData, transactionData.ParametersAsJson);
        }
    }
}
