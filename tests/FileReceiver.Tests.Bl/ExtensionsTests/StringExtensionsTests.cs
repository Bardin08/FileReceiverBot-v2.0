using FileReceiver.Common.Extensions;

using FluentAssertions;

using Xunit;

namespace FileReceiver.Tests.Bl.ExtensionsTests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(true, "/command")]
        [InlineData(true, "/command params")]
        public void IsPossibleCommand_ShouldReturnTrue_WhenTextIsCommand(bool expected, string input)
        {
            // Arrange

            // Act
            var isCommand = input.IsPossibleCommand();

            // Assert
            isCommand.Should().Be(expected);
        }

        [Theory]
        [InlineData(false, "")]
        [InlineData(false, "/")]
        [InlineData(false, "text")]
        public void IsPossibleCommand_ShouldReturnFalse_WhenTextIsNotACommand(bool expected, string input)
        {
            // Arrange

            // Act
            var isCommand = input.IsPossibleCommand();

            // Assert
            isCommand.Should().Be(expected);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("/command", "/command")]
        [InlineData("/command", "/command with some text")]
        [InlineData("/command", "/command.text")]
        [InlineData("/command_ext", "/command_ext")]
        [InlineData("/command_ext", "/command_ext!some additional text")]
        public void GetCommandFromMessage_ShouldReturnCommand_WhenTextIsNotACommand(string expected, string input)
        {
            // Arrange

            // Act
            var isCommand = input.GetCommandFromMessage();

            // Assert
            isCommand.Should().Be(expected);
        }
    }
}
