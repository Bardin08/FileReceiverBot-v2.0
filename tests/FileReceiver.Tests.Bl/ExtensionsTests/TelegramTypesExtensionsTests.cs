using System;
using System.Collections.Generic;

using FileReceiver.Common.Extensions;
using FileReceiver.Tests.Bl.ExtensionsTests.Data;

using FluentAssertions;

using Telegram.Bot.Types;

using Xunit;

namespace FileReceiver.Tests.Bl.ExtensionsTests
{
    public class TelegramTypesExtensionsTests
    {
        [Theory]
        [ClassData(typeof(GetUserTgIdTestsDataForValidUpdateObject))]
        public void GetTgUserId_ShouldReturnUserId_WhenUpdateObjectValid(long expected, Update update)
        {
            // Arrange

            // Act
            var id = update.GetTgUserId();

            // Assert
            expected.Should().Be(id);
        }

        [Theory]
        [MemberData(nameof(DataForInvalidObjects))]
        public void GetTgUserId_ShouldReturnNull(Update update)
        {
            // Arrange

            // Act
            Action getUserIdAction = () => update.GetTgUserId();

            // Assert
            getUserIdAction.Should().Throw<ArgumentOutOfRangeException>();
        }

        public static IEnumerable<object[]> DataForInvalidObjects()
        {
            yield return new object[] { null };
            yield return new object[] { new Update() };
        }
    }
}
