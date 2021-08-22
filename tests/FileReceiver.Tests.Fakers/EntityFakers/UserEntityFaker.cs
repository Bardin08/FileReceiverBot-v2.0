using System;

using Bogus;

using FileReceiver.Common.Extensions;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;
using FileReceiver.Tests.Fakers.Configurations;

namespace FileReceiver.Tests.Fakers.EntityFakers
{
    internal static class UserEntityFaker
    {
        internal static Faker<UserEntity> GetUserEntityFaker(int seed = FakersConstants.FakersSeed)
        {
            return new Faker<UserEntity>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, x => x.Random.Long(FakersConstants.MinUserId, FakersConstants.MaxUserId))
                .RuleFor(x => x.TelegramTag, x => x.Person.UserName)
                .RuleFor(x => x.FirstName, x => x.Person.FirstName)
                .RuleFor(x => x.LastName, x => x.Person.LastName)
                .RuleFor(x => x.SecretWordHash, x => x.Internet.Password().CreateHash())
                .RuleFor(x => x.RegistrationState, RegistrationStateDb.RegistrationComplete)
                .RuleFor(x => x.RegistrationEndTimestamp,
                    x => x.Date.PastOffset(refDate: FakersConstants.RegistrationBeginTimeOffset));
        }

        public static UserEntity GenerateUserEntity(int seed = FakersConstants.FakersSeed)
        {
            return GetUserEntityFaker(seed)
                .Generate();
        }
    }
}
