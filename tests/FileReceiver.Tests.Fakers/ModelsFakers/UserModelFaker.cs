using Bogus;

using FileReceiver.Common.Enums;
using FileReceiver.Common.Extensions;
using FileReceiver.Common.Models;
using FileReceiver.Tests.Fakers.Configurations;

namespace FileReceiver.Tests.Fakers.ModelsFakers
{
    public static class UserModelFaker
    {
        internal static Faker<UserModel> GetUserModelFaker(int seed = FakersConstants.FakersSeed)
        {
            return new Faker<UserModel>()
                .UseSeed(seed)
                .RuleFor(x => x.Id, x => x.Random.Long(FakersConstants.MinUserId, FakersConstants.MaxUserId))
                .RuleFor(x => x.TelegramTag, x => x.Person.UserName)
                .RuleFor(x => x.FirstName, x => x.Person.FirstName)
                .RuleFor(x => x.LastName, x => x.Person.LastName)
                .RuleFor(x => x.SecretWordHash, x => x.Internet.Password().CreateHash())
                .RuleFor(x => x.RegistrationState, RegistrationState.RegistrationComplete)
                .RuleFor(x => x.RegistrationEndTimestamp,
                    x => x.Date.PastOffset(refDate: FakersConstants.RegistrationBeginTimeOffset));
        }

        public static UserModel GenerateUserModel(int seed = FakersConstants.FakersSeed)
        {
            return GetUserModelFaker(seed).Generate();
        }
    }
}
