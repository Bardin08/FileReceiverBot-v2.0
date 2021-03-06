using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.Factories;
using FileReceiver.Bl.Impl.Handlers.CallbackQuery;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Telegram.Bot.Types;

using Xunit;

namespace FileReceiver.Tests.Bl.FactoriesTests
{
    public class CallbackQueryHandlerFactoryTests
    {
        private readonly CallbackQueryHandlerFactory _sut;
        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();

        private readonly IBotMessagesService _botMessagesService = Substitute.For<IBotMessagesService>();
        private readonly IUserService _userService = Substitute.For<IUserService>();
        private readonly IBotTransactionService _transactionService = Substitute.For<IBotTransactionService>();

        private readonly IUpdateHandlerFactory _updateHandlerFactory = Substitute.For<IUpdateHandlerFactory>();

        private readonly ILogger<EditProfileCallbackQueryHandler> _logger = Substitute.For<ILogger<EditProfileCallbackQueryHandler>>();

        public CallbackQueryHandlerFactoryTests()
        {
            _sut = new CallbackQueryHandlerFactory(_serviceProvider);
        }

        [Fact]
        public void CreateCallbackQueryHandler_ShouldReturnEditProfileCallbackQueryHandler_WhenCallbackForProfileEditing()
        {
            // Arrange
            var callbackQuery = new CallbackQuery()
            {
                Data = "profile-",
            };
            var callbackHandler = new EditProfileCallbackQueryHandler(
                _botMessagesService,
                _userService,
                _transactionService,
                _logger);
            _serviceProvider.GetService<EditProfileCallbackQueryHandler>().Returns(callbackHandler);

            // Act
            var handler = _sut.CreateCallbackHandler(callbackQuery);

            // Assert
            handler.Should().Be(callbackHandler);
        }

        [Fact]
        public void CreateCallbackQueryHandler_ShouldReturnFileReceivingSessionCallbackQueryHandler_WhenCallbackForFileReceivingSession()
        {
            // Arrange
            var callbackQuery = new CallbackQuery()
            {
                Data = "fr-session",
            };
            var callbackHandler = new FileReceivingSessionCallbackQueryHandler(_updateHandlerFactory);
            _serviceProvider.GetService<FileReceivingSessionCallbackQueryHandler>().Returns(callbackHandler);

            // Act
            var handler = _sut.CreateCallbackHandler(callbackQuery);

            // Assert
            handler.Should().Be(callbackHandler);
        }
    }
}
