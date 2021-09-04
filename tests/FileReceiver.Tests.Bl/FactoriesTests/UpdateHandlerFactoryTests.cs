using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.Factories;
using FileReceiver.Bl.Impl.Handlers.TelegramUpdate;
using FileReceiver.Common.Enums;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Integrations.Mega.Abstract;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Telegram.Bot;

using Xunit;

namespace FileReceiver.Tests.Bl.FactoriesTests
{
    public class UpdateHandlerFactoryTests
    {
        private readonly IUpdateHandlerFactory _sut;
        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();

        private readonly IUserService _service = Substitute.For<IUserService>();
        private readonly IBotMessagesService _botMessagesService = Substitute.For<IBotMessagesService>();
        private readonly IFileReceivingSessionService _receivingSessionService = Substitute.For<IFileReceivingSessionService>();

        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

        private readonly ITelegramBotClient _botClient = Substitute.For<ITelegramBotClient>();
        private readonly IMegaApiClient _megaApiClient = Substitute.For<IMegaApiClient>();


        public UpdateHandlerFactoryTests()
        {
            _sut = new UpdateHandlerFactory(_serviceProvider);
        }

        [Fact]
        public void CreateUpdateHandler_ShouldReturnDefaultUpdateHandler_WhenUpdateTypeUnknown()
        {
            // Arrange
            var updateHandler = new DefaultUpdateHandler(_botMessagesService);
            _serviceProvider.GetService<DefaultUpdateHandler>().Returns(updateHandler);

            // Act
            var handler = _sut.CreateUpdateHandler(TransactionType.Unknown);

            // Assert
            handler.Should().Be(updateHandler);
        }

        [Fact]
        public void CreateUpdateHandler_ShouldReturnRegistrationUpdateHandler_WhenUpdateTypeRegistration()
        {
            // Arrange
            var updateHandler = new RegistrationUpdateHandler(_botMessagesService, _service,
                _transactionRepository);
            _serviceProvider.GetService<RegistrationUpdateHandler>().Returns(updateHandler);

            // Act
            var handler = _sut.CreateUpdateHandler(TransactionType.Registration);

            // Assert
            handler.Should().Be(updateHandler);
        }

        [Fact]
        public void CreateUpdateHandler_ShouldReturnProfileEditUpdateHandler_WhenUpdateTypeEditProfile()
        {
            // Arrange
            var updateHandler = new EditProfileUpdateHandler(_botMessagesService, _userRepository, _transactionRepository);
            _serviceProvider.GetService<EditProfileUpdateHandler>().Returns(updateHandler);

            // Act
            var handler = _sut.CreateUpdateHandler(TransactionType.EditProfile);

            // Assert
            handler.Should().Be(updateHandler);
        }

        [Fact]
        public void CreateUpdateHandler_ShouldReturnFileReceivingSessionCreatingUpdateHandler_WhenUpdateTypeFileReceivingSessionCreating()
        {
            // Arrange
            ILogger<FileReceivingSessionCreatingUpdateHandler> logger =
                Substitute.For<ILogger<FileReceivingSessionCreatingUpdateHandler>>();
            var updateHandler = new FileReceivingSessionCreatingUpdateHandler(_botMessagesService,
                _receivingSessionService, _botClient, _megaApiClient, logger);
            _serviceProvider.GetService<FileReceivingSessionCreatingUpdateHandler>().Returns(updateHandler);

            // Act
            var handler = _sut.CreateUpdateHandler(TransactionType.FileReceivingSessionCreating);

            // Assert
            handler.Should().Be(updateHandler);
        }

        [Fact]
        public void CreateUpdateHandler_ShouldReturnDefaultUpdateHandler_WhenUpdateTypeDefault()
        {
            // Arrange
            var updateHandler = new DefaultUpdateHandler(_botMessagesService);
            _serviceProvider.GetService<DefaultUpdateHandler>().Returns(updateHandler);

            // Act
            var handler = _sut.CreateUpdateHandler((TransactionType)11);

            // Assert
            handler.Should().Be(updateHandler);
        }
    }
}
