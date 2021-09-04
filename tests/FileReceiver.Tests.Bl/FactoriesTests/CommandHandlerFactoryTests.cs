using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.Factories;
using FileReceiver.Bl.Impl.Handlers.Commands;
using FileReceiver.Common.Constants;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace FileReceiver.Tests.Bl.FactoriesTests
{
    public class CommandHandlerFactoryTests
    {
        private readonly ICommandHandlerFactory _sut;
        private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();

        private readonly IBotMessagesService _botMessagesService = Substitute.For<IBotMessagesService>();
        private readonly IFileReceivingSessionService _receivingSessionService = Substitute.For<IFileReceivingSessionService>();
        private readonly IUserService _userService = Substitute.For<IUserService>();
        private readonly IBotTransactionService _transactionService = Substitute.For<IBotTransactionService>();

        private readonly ILogger<StartCommandHandler> _loggerStart = Substitute.For<ILogger<StartCommandHandler>>();
        private readonly ILogger<RegisterCommandHandler> _loggerRegister = Substitute.For<ILogger<RegisterCommandHandler>>();
        private readonly ILogger<CancelCommandHandler> _loggerCancel = Substitute.For<ILogger<CancelCommandHandler>>();
        private readonly ILogger<ProfileCommandHandler> _loggerProfile = Substitute.For<ILogger<ProfileCommandHandler>>();
        private readonly ILogger<ProfileEditCommandHandler> _loggerProfileEdit = Substitute.For<ILogger<ProfileEditCommandHandler>>();
        private readonly ILogger<StartReceivingCommandHandler> _loggerStartReceiving = Substitute.For<ILogger<StartReceivingCommandHandler>>();
        private readonly ILogger<DefaultCommandHandler> _loggerDefaultCommand = Substitute.For<ILogger<DefaultCommandHandler>>();

        public CommandHandlerFactoryTests()
        {
            _sut = new CommandHandlerFactory(_serviceProvider);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnStartCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new StartCommandHandler(_userService, _botMessagesService, _sut, _loggerStart);
            _serviceProvider.GetService<StartCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.Start);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnRegisterCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new RegisterCommandHandler(_botMessagesService, _transactionService,
                _userService, _loggerRegister);
            _serviceProvider.GetService<RegisterCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.Register);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnCancelCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new CancelCommandHandler(_transactionService, _botMessagesService, _loggerCancel);
            _serviceProvider.GetService<CancelCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.Cancel);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnAbortCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new CancelCommandHandler(_transactionService, _botMessagesService, _loggerCancel);
            _serviceProvider.GetService<CancelCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.Abort);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnProfileCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new ProfileCommandHandler(_botMessagesService, _userService, _loggerProfile);
            _serviceProvider.GetService<ProfileCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.Profile);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnProfileEditCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new ProfileEditCommandHandler(_botMessagesService, _userService, _loggerProfileEdit);
            _serviceProvider.GetService<ProfileEditCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.ProfileEdit);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnStartReceivingCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new StartReceivingCommandHandler(
                _botMessagesService,
                _userService,
                _receivingSessionService,
                _loggerStartReceiving);
            _serviceProvider.GetService<StartReceivingCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(Commands.StartReceiving);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnDefaultCommandHandler_WhenCommandUnsupported()
        {
            // Arrange
            const string command = "/some_random_command";
            var commandHandler = new DefaultCommandHandler(_botMessagesService, _loggerDefaultCommand);
            _serviceProvider.GetService<DefaultCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler(command);

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnNull_WhenNotACommand()
        {
            // Arrange
            const string command = "some_random_text";

            // Act
            var handler = _sut.CreateCommandHandler(command);

            // Assert
            handler.Should().BeNull();
        }
    }
}
