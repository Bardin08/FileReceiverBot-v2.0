using System;

using AutoMapper;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.Factories;
using FileReceiver.Bl.Impl.Handlers.Commands;
using FileReceiver.Dal.Abstract.Repositories;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

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

        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
        private readonly ITransactionRepository _transactionRepository = Substitute.For<ITransactionRepository>();

        private readonly IMapper _mapper = Substitute.For<IMapper>();

        public CommandHandlerFactoryTests()
        {
            _sut = new CommandHandlerFactory(_serviceProvider);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnStartCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new StartCommandHandler(_userRepository, _sut, _botMessagesService);
            _serviceProvider.GetService<StartCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/start");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnRegisterCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new RegisterCommandHandler(_botMessagesService, _transactionRepository,
                _userRepository, _mapper);
            _serviceProvider.GetService<RegisterCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/register");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnCancelCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new CancelCommandHandler(_transactionRepository, _botMessagesService);
            _serviceProvider.GetService<CancelCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/cancel");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnAbortCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new CancelCommandHandler(_transactionRepository, _botMessagesService);
            _serviceProvider.GetService<CancelCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/abort");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnProfileCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new ProfileCommandHandler(_botMessagesService, _userRepository);
            _serviceProvider.GetService<ProfileCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/profile");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnProfileEditCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new ProfileEditCommandHandler(_botMessagesService, _userRepository);
            _serviceProvider.GetService<ProfileEditCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/profile_edit");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnStartReceivingCommandHandler_WhenCommandValid()
        {
            // Arrange
            var commandHandler = new StartReceivingCommandHandler(_receivingSessionService, _botMessagesService);
            _serviceProvider.GetService<StartReceivingCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/start_receiving");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnDefaultCommandHandler_WhenCommandUnsupported()
        {
            // Arrange
            var commandHandler = new DefaultCommandHandler(_botMessagesService);
            _serviceProvider.GetService<DefaultCommandHandler>().Returns(commandHandler);

            // Act
            var handler = _sut.CreateCommandHandler("/some_random_command");

            // Assert
            handler.Should().Be(commandHandler);
        }

        [Fact]
        public void CreateCommandHandler_ShouldReturnNull_WhenNotACommand()
        {
            // Arrange

            // Act
            var handler = _sut.CreateCommandHandler("some_random_text");

            // Assert
            handler.Should().BeNull();
        }
    }
}
