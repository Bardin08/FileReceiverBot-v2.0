using System;

using Bogus;

using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;
using FileReceiver.Tests.Bl.Constants;

namespace FileReceiver.Tests.Bl.ServicesTests.Data.EntityFakers
{
    internal static class UserEntityFaker
    {
        private const long MinUserId = 100000;
        private const long MaxUserId = 999999;
        private static readonly DateTimeOffset RegistrationBeginTimeOffset = DateTimeOffset.UtcNow.AddHours(-12);

        internal static Faker<UserEntity> GetUserEntityFaker(int seed = FakersConstants.FakersSeed)
        {
            return new Faker<UserEntity>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, x => x.Random.Long(MinUserId, MaxUserId))
                .RuleFor(x => x.TelegramTag, x => x.Person.UserName)
                .RuleFor(x => x.FirstName, x => x.Person.FirstName)
                .RuleFor(x => x.LastName, x => x.Person.LastName)
                .RuleFor(x => x.SecretWordHash, x => x.Internet.Password().CreateHash())
                .RuleFor(x => x.RegistrationState, RegistrationStateDb.RegistrationComplete)
                .RuleFor(x => x.RegistrationEndTimestamp,
                    x => x.Date.PastOffset(refDate: RegistrationBeginTimeOffset));
        }

        internal static UserEntity GenerateUser(int seed = FakersConstants.FakersSeed)
        {
            return GetUserEntityFaker(seed)
                .Generate();
        }
    }
}
