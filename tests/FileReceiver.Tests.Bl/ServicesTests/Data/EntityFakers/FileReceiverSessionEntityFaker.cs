using System;

using Bogus;

using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;
using FileReceiver.Tests.Bl.Constants;

namespace FileReceiver.Tests.Bl.ServicesTests.Data.EntityFakers
{
    internal static class FileReceiverSessionEntityFaker
    {
        private const int MinFilesPerSession = 10;
        private const int MaxFilesPerSession = 100;

        internal static Faker<FileReceivingSessionEntity> GetFileReceiverSessionEntityFaker(
            int seed = FakersConstants.FakersSeed)
        {
            var user = UserEntityFaker.GetUserEntityFaker(seed).Generate();
            return new Faker<FileReceivingSessionEntity>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, Guid.NewGuid())
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.User, user)
                .RuleFor(x => x.SessionState, x => x.PickRandom<FileReceivingSessionStateDb>())
                .RuleFor(x => x.FilesReceived, x => x.Random.Int(MinFilesPerSession, MaxFilesPerSession))
                .RuleFor(x => x.MaxFiles, x => x.Random.Int(MinFilesPerSession, MaxFilesPerSession))
                .RuleFor(x => x.Storage, x => x.PickRandom<FileStorageTypeDb>())
                .RuleFor(x => x.Constrains, new ConstraintsModel().ConstraintsAsJson)
                .RuleFor(x => x.CreateData, x => x.Date.Past());
        }

        internal static FileReceivingSessionEntity GenerateFileReceivingSessionEntityWithRandomState(
            int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionEntityFaker(seed).Generate();
        }

        internal static FileReceivingSessionEntity GenerateForCreateSessionMethod(int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionEntityFaker(seed)
                .RuleFor(x => x.SessionState, FileReceivingSessionStateDb.FileSizeConstraintSet)
                .RuleFor(x => x.MaxFiles, 50)
                .RuleFor(x => x.CreateData, DateTimeOffset.UtcNow)
                .Generate();
        }

        internal static FileReceivingSessionEntity GenerateForSetConstraintMethods(int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionEntityFaker(seed)
                .RuleFor(x => x.SessionState, FileReceivingSessionStateDb.FileSizeConstraintSet)
                .RuleFor(x => x.MaxFiles, 50)
                .RuleFor(x => x.CreateData, DateTimeOffset.UtcNow)
                .Generate();
        }
    }
}
