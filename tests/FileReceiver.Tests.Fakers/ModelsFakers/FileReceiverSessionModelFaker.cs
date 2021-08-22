using System;

using Bogus;

using FileReceiver.Common.Enums;
using FileReceiver.Common.Models;
using FileReceiver.Tests.Fakers.Configurations;

namespace FileReceiver.Tests.Fakers.ModelsFakers
{
    public class FileReceiverSessionModelFaker
    {
        internal static Faker<FileReceivingSessionModel> GetFileReceiverSessionModelFaker(
            int seed = FakersConstants.FakersSeed)
        {
            var user = UserModelFaker.GetUserModelFaker(seed).Generate();
            return new Faker<FileReceivingSessionModel>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, Guid.NewGuid())
                .RuleFor(x => x.UserId, user.Id)
                .RuleFor(x => x.User, user)
                .RuleFor(x => x.SessionState, x => x.PickRandom<FileReceivingSessionState>())
                .RuleFor(x => x.FilesReceived, x => x.Random.Int(FakersConstants.MinFilesPerSession, FakersConstants.MaxFilesPerSession))
                .RuleFor(x => x.MaxFiles, x => x.Random.Int(FakersConstants.MinFilesPerSession, FakersConstants.MaxFilesPerSession))
                .RuleFor(x => x.Storage, x => x.PickRandom<FileStorageType>())
                .RuleFor(x => x.Constrains, new ConstraintsModel())
                .RuleFor(x => x.CreateData, x => x.Date.Past());
        }

        public static FileReceivingSessionModel GenerateFileReceivingSessionModel(int seed = FakersConstants.FakersSeed)
        {
            return GetFileReceiverSessionModelFaker(seed).Generate();
        }
    }
}
