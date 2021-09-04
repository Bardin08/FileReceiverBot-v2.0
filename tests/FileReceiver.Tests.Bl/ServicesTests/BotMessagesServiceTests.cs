using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.Services;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using Xunit;

namespace FileReceiver.Tests.Bl.ServicesTests
{
    public class BotMessagesServiceTests
    {
        private readonly IBotMessagesService _sut;

        private readonly ITelegramBotClient _botClient = Substitute.For<ITelegramBotClient>();
        private readonly ILogger<BotMessagesService> _logger = Substitute.For<ILogger<BotMessagesService>>();
        public BotMessagesServiceTests()
        {
            _sut = new BotMessagesService(_botClient, _logger);
        }

        [Theory]
        [InlineData(912903)]
        public async Task SendMenuAsync_ShouldReturnNothing_WhenUserIdValid(long userId)
        {
            // Arrange

            // Act
            await _sut.SendMenuAsync(userId);

            // Assert
            await _botClient.Received(1).SendTextMessageAsync(userId, "This is a bot menu...");
        }

        [Theory]
        [InlineData(912903, "Description")]
        public async Task SendErrorAsync_ShouldReturnNothing_WhenUserIdValid(long userId, string errorDescription)
        {
            // Arrange

            // Act
            await _sut.SendErrorAsync(userId, errorDescription);

            // Assert
            await _botClient.Received(1).SendTextMessageAsync(userId,
                $"Sorry, an error happen. Error description: {errorDescription}");
        }

        [Theory]
        [InlineData(912903, "message")]
        public async Task SendTextMessageAsync_ShouldReturnNothing_WhenUserIdValid(long userId, string message)
        {
            // Arrange

            // Act
            await _sut.SendTextMessageAsync(userId, message);

            // Assert
            await _botClient.Received(1).SendTextMessageAsync(userId, message, ParseMode.Markdown);
        }

        [Theory]
        [InlineData(912903, "message", null)]
        public async Task SendMessageWithKeyboardAsync_ShouldReturnNothing_WhenUserIdValid(long userId, string message,
            IReplyMarkup keyboard)
        {
            // Arrange

            // Act
            await _sut.SendMessageWithKeyboardAsync(userId, message, keyboard);

            // Assert
            await _botClient.Received(1).SendTextMessageAsync(userId, message, ParseMode.Markdown, replyMarkup: keyboard);
        }

        [Theory]
        [InlineData(912903, "message")]
        public async Task SendNotSupportedAsync_ShouldReturnNothing_WhenUserIdValid(long userId,
            string notSupportedActionName)
        {
            // Arrange

            // Act
            await _sut.SendNotSupportedAsync(userId, notSupportedActionName);

            // Assert
            await _botClient.SendTextMessageAsync(userId,
                $"Sorry, this action: *{notSupportedActionName}* is not supported now", ParseMode.Markdown);
        }
    }
}
