using System;

using Bogus;

using FileReceiver.Common.Models;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;
using FileReceiver.Tests.Fakers.Configurations;

namespace FileReceiver.Tests.Fakers.EntityFakers
{
    public static class FileReceiverSessionEntityFaker
    {
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
                .RuleFor(x => x.FilesReceived, x => x.Random.Int(FakersConstants.MinFilesPerSession, FakersConstants.MaxFilesPerSession))
                .RuleFor(x => x.MaxFiles, x => x.Random.Int(FakersConstants.MinFilesPerSession, FakersConstants.MaxFilesPerSession))
                .RuleFor(x => x.Storage, x => x.PickRandom<FileStorageTypeDb>())
                .RuleFor(x => x.Constrains, new ConstraintsModel().ConstraintsAsJson)
                .RuleFor(x => x.CreateData, x => x.Date.Past());
        }

        public static FileReceivingSessionEntity GenerateFileReceivingSessionEntityWithRandomState(
            int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionEntityFaker(seed).Generate();
        }

        public static FileReceivingSessionEntity GenerateForCreateSessionMethod(int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionEntityFaker(seed)
                .RuleFor(x => x.SessionState, FileReceivingSessionStateDb.FileSizeConstraintSet)
                .RuleFor(x => x.MaxFiles, 50)
                .RuleFor(x => x.CreateData, DateTimeOffset.UtcNow)
                .Generate();
        }

        public static FileReceivingSessionEntity GenerateForSetConstraintMethods(int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionEntityFaker(seed)
                .RuleFor(x => x.SessionState, FileReceivingSessionStateDb.FileSizeConstraintSet)
                .RuleFor(x => x.MaxFiles, 50)
                .RuleFor(x => x.CreateData, DateTimeOffset.UtcNow)
                .Generate();
        }
    }
}
