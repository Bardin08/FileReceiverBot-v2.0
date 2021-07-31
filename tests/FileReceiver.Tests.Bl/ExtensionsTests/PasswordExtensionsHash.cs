using System.Runtime.InteropServices;

using FileReceiver.Common.Extensions;

using FluentAssertions;

using Xunit;

namespace FileReceiver.Tests.Bl.ExtensionsTests
{
    public class PasswordExtensionsHash
    {
        [Theory]
        [InlineData("password")]
        public void CreateHash_ShouldReturnStringHashCode_WhenPasswordIsText(string password)
        {
            // Arrange

            // Act
            var hash = password.CreateHash();
            var validationResult = password.ValidatePassword(hash);

            // Assert
            validationResult.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, "1000:zYNm0Co2QKSnW89oNb5DwUFI2tCWnHad:ZmowQzAZQK9hRmlYMsx/4rsHKx/rfR0H", "password")]
        public void CreateHash_ShouldReturnTrue_WhenPasswordValid(bool expected, string hash, string password)
        {
            // Arrange

            // Act
            var result = password.ValidatePassword(hash);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, "1000:zYNm0Co2QKSnW89oNb5DwUFI2tCWnHad:ZmowQzAZQK9hRmlYMsx/4rsHKx/rfR0H", "password-code")]
        public void CreateHash_ShouldReturnFalse_WhenPasswordInvalid(bool expected, string hash, string password)
        {
            // Arrange

            // Act
            var result = password.ValidatePassword(hash);

            // Assert
            result.Should().Be(false);
        }
    }
}
