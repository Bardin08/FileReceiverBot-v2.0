#pragma warning disable 1998

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.AutoMapper.Profiles;
using FileReceiver.Bl.Impl.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Exceptions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Tests.Fakers.EntityFakers;
using FileReceiver.Tests.Fakers.ModelsFakers;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace FileReceiver.Tests.Bl.ServicesTests
{
    public class FileReceivingSessionServiceTests
    {
        private readonly IFileReceivingSessionService _sut;

        private readonly IBotMessagesService _botMessagesService = Substitute.For<IBotMessagesService>();
        private readonly IBotTransactionService _transactionService = Substitute.For<IBotTransactionService>();

        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

        private readonly IFileReceivingSessionRepository _receivingSessionRepository =
            Substitute.For<IFileReceivingSessionRepository>();

        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IMapper _internalMapper;

        public FileReceivingSessionServiceTests()
        {
            _internalMapper = new MapperConfiguration(opt =>
            {
                opt.AddProfiles(
                    new List<Profile>()
                    {
                        new RegistrationProfile(),
                        new TransactionsProfile(),
                        new FileReceivingSessionProfile(),
                    });
            }).CreateMapper();

            _sut = new FileReceivingSessionService(
                _botMessagesService,
                _userRepository,
                _transactionService,
                _receivingSessionRepository,
                _mapper);
        }

        [Fact]
        public async Task CreateFileReceivingSessionAsync_ShouldReturnNothing_WhenUserWithGivenIdExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForCreateSessionMethod();
            var transaction = TransactionModelFaker.GenerateNewFileSessionTransactionModel(sessionEnt.UserId);
            var transactionEnt = TransactionEntityFaker.GenerateNewFileSessionTransactionEntity(sessionEnt.User);
            var userEnt = sessionEnt.User;

            _userRepository.CheckIfUserExistsAsync(Arg.Any<long>()).Returns(true);
            _mapper.Map<FileReceivingSessionEntity>(Arg.Any<FileReceivingSessionModel>()).Returns(sessionEnt);
            _mapper.Map<TransactionEntity>(Arg.Any<TransactionModel>()).Returns(transactionEnt);
            _receivingSessionRepository.AddAsync(sessionEnt).Returns(sessionEnt);
            _transactionService.Add(transaction).Returns(transaction);

            // Act
            await _sut.CreateFileReceivingSessionAsync(userEnt.Id);

            // Assert
            await _userRepository.Received(1).CheckIfUserExistsAsync(userEnt.Id);
            _mapper.Received(1).Map<FileReceivingSessionEntity>(Arg.Any<FileReceivingSessionModel>());
            await _receivingSessionRepository.Received(1).AddAsync(Arg.Any<FileReceivingSessionEntity>());
            await _transactionService.Received(1).Add(Arg.Any<TransactionModel>());
        }

        [Fact]
        public async Task CreateFileReceivingSessionAsync_ShouldReturnAnException_WhenUserWithGivenIdIsNotExists()
        {
            // Arrange
            var userEnt = UserModelFaker.GenerateUserModel();
            _userRepository.CheckIfUserExistsAsync(Arg.Any<long>()).Returns(false);

            // Act
            Func<Task> createSession = async () => await _sut.CreateFileReceivingSessionAsync(userEnt.Id);

            // Assert
            createSession.Should().Throw<UserProfileNotFoundException>()
                .And.TelegramUserId.Should().Be(userEnt.Id);
        }

        [Fact]
        public async Task SetFileSizeConstraintAsync_ShouldReturnNothing_WhenSessionEntityExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var sessionModel = _internalMapper.Map<FileReceivingSessionModel>(sessionEnt);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _mapper.Map<FileReceivingSessionModel>(Arg.Any<FileReceivingSessionEntity>()).Returns(sessionModel);

            // Act
            await _sut.SetFileSizeConstraintAsync(sessionEnt.Id);

            // Assert
            _mapper.Received(1).Map<FileReceivingSessionModel>(Arg.Any<FileReceivingSessionEntity>());
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task SetFileSizeConstraintAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(
                Task.FromResult<FileReceivingSessionEntity>(null));

            // Act
            Func<Task> getSessionAction = async () => await _sut.SetFileSizeConstraintAsync(sessionId);

            // Assert
            getSessionAction.Should().Throw<FileReceivingSessionNotFound>()
                .And.DesiredSessionId.Should().Be(sessionId);
        }

        [Fact]
        public async Task SetFileNameConstraintAsync_ShouldReturnNothing_WhenSessionEntityExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var sessionModel = _internalMapper.Map<FileReceivingSessionModel>(sessionEnt);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _mapper.Map<FileReceivingSessionModel>(Arg.Any<FileReceivingSessionEntity>()).Returns(sessionModel);

            // Act
            await _sut.SetFileNameConstraintAsync(sessionEnt.Id, "regex");

            // Assert
            _mapper.Received(1).Map<FileReceivingSessionModel>(Arg.Any<FileReceivingSessionEntity>());
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task SetFileNameConstraintAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(
                Task.FromResult<FileReceivingSessionEntity>(null));

            // Act
            Func<Task> getSessionAction = async () => await _sut.SetFileNameConstraintAsync(sessionId, "regex");

            // Assert
            getSessionAction.Should().Throw<FileReceivingSessionNotFound>()
                .And.DesiredSessionId.Should().Be(sessionId);
        }

        [Fact]
        public async Task SetFileExtensionConstraintAsync_ShouldReturnNothing_WhenSessionEntityExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var sessionModel = _internalMapper.Map<FileReceivingSessionModel>(sessionEnt);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _mapper.Map<FileReceivingSessionModel>(Arg.Any<FileReceivingSessionEntity>()).Returns(sessionModel);

            // Act
            await _sut.SetFileExtensionConstraintAsync(sessionEnt.Id, "");

            // Assert
            _mapper.Received(1).Map<FileReceivingSessionModel>(Arg.Any<FileReceivingSessionEntity>());
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task SetFileExtensionConstraintAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(
                Task.FromResult<FileReceivingSessionEntity>(null));

            // Act
            Func<Task> getSessionAction = async () => await _sut.SetFileExtensionConstraintAsync(sessionId, "");

            // Assert
            getSessionAction.Should().Throw<FileReceivingSessionNotFound>()
                .And.DesiredSessionId.Should().Be(sessionId);
        }

        [Fact]
        public async Task SetSessionMaxFilesConstraintAsync_ShouldReturnNothing_WhenSessionEntityExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);

            // Act
            await _sut.SetSessionMaxFilesConstraintAsync(sessionEnt.User.Id, sessionEnt.Id, 10);

            // Assert
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task SetSessionMaxFilesConstraintAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>())
                .Returns(Task.FromResult<FileReceivingSessionEntity>(null));
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);

            // Act
            Func<Task> setConstraint = async ()
                => await _sut.SetSessionMaxFilesConstraintAsync(sessionEnt.User.Id, sessionEnt.Id, 10);

            // Assert
            setConstraint.Should().Throw<FileReceivingSessionNotFound>()
                .And.DesiredSessionId.Should().Be(sessionEnt.Id);
        }

        [Fact]
        public async Task SetFilesStorageAsync_ShouldReturnNothing_WhenSessionEntityExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var session = FileReceiverSessionModelFaker.GenerateFileReceivingSessionModel();
            var transaction = TransactionModelFaker.GenerateTransactionEntityWithFileReceivingSessionId(
                session.UserId, sessionEnt.Id);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating).Returns(transaction);

            // Act
            await _sut.SetFilesStorageAsync(sessionEnt.User.Id, FileStorageType.None);

            // Assert
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task SetFilesStorageAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating)
                .Returns(Task.FromResult<TransactionModel>(null));

            // Act
            Func<Task> setStorage = async () => await _sut.SetFilesStorageAsync(sessionEnt.User.Id, FileStorageType.None);

            // Assert
            var ex = setStorage.Should().Throw<FileReceivingSessionActionErrorException>();
            ex.Which.TelegramUserId.Should().Be(sessionEnt.UserId);
            ex.Which.SessionAction.Should().Be(nameof(_sut.SetFilesStorageAsync));
            await _botMessagesService.Received(1).SendErrorAsync(sessionEnt.UserId,
                "Session wasn't found. Use command /cancel and then /start_receiving to create a new one");
        }

        [Fact]
        public async Task ExecuteSessionAsync_ShouldReturnNothing_WhenSessionEntityExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var session = FileReceiverSessionModelFaker.GenerateFileReceivingSessionModel();
            var transaction = TransactionModelFaker
                .GenerateTransactionEntityWithFileReceivingSessionId(session.UserId, sessionEnt.Id);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating)
                .Returns(transaction);

            // Act
            var sessionIdFromMethod = await _sut.ExecuteSessionAsync(sessionEnt.User.Id);

            // Assert
            sessionIdFromMethod.Should().Be(sessionEnt.Id.ToString());

            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task ExecuteSessionAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating)
                .Returns(Task.FromResult<TransactionModel>(null));

            // Act
            Func<Task> setStorage = async () => await _sut.ExecuteSessionAsync(sessionEnt.User.Id);

            // Assert
            var ex = setStorage.Should().Throw<FileReceivingSessionActionErrorException>();
            ex.Which.TelegramUserId.Should().Be(sessionEnt.UserId);
            ex.Which.SessionAction.Should().Be(nameof(_sut.ExecuteSessionAsync));
            await _botMessagesService.Received(1).SendErrorAsync(sessionEnt.UserId,
                "Session wasn't found. Use command /cancel and then /start_receiving to create a new one");
        }

        [Fact]
        public async Task StopSessionAsync_ShouldReturnNothing_WhenSessionEntityExistsAndOwnsTheSession()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var session = FileReceiverSessionModelFaker.GenerateFileReceivingSessionModel();
            var transaction = TransactionModelFaker
                .GenerateTransactionEntityWithFileReceivingSessionId(session.UserId, sessionEnt.Id);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating)
                .Returns(transaction);

            // Act
            await _sut.StopSessionAsync(sessionEnt.User.Id);

            // Assert
            await _botMessagesService.Received(1).SendTextMessageAsync(sessionEnt.UserId, "Session stopped");
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
            await _receivingSessionRepository.Received(1).UpdateAsync(Arg.Any<FileReceivingSessionEntity>());
        }

        [Fact]
        public async Task StopSessionAsync_ShouldReturnNothing_WhenSessionEntityExistsAndNotOwnsTheSession()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            var userId = sessionEnt.UserId + 1;
            var session = FileReceiverSessionModelFaker.GenerateFileReceivingSessionModel();
            var transaction = TransactionModelFaker
                .GenerateTransactionEntityWithFileReceivingSessionId(session.UserId, sessionEnt.Id);

            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);
            _receivingSessionRepository.UpdateAsync(Arg.Any<FileReceivingSessionEntity>()).Returns(Task.CompletedTask);
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating)
                .Returns(transaction);

            // Act
            await _sut.StopSessionAsync(userId);

            // Assert
            await _botMessagesService.Received(1).SendErrorAsync(userId, "You can stop only yours sessions");
            await _receivingSessionRepository.Received(1).GetByIdAsync(Arg.Any<Guid>());
        }

        [Fact]
        public async Task StopSessionAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateForSetConstraintMethods();
            _transactionService.Get(Arg.Any<long>(), TransactionType.FileReceivingSessionCreating)
                .Returns(Task.FromResult<TransactionModel>(null));

            // Act
            Func<Task> setStorage = async () => await _sut.StopSessionAsync(sessionEnt.User.Id);

            // Assert
            var ex = setStorage.Should().Throw<FileReceivingSessionActionErrorException>();
            ex.Which.TelegramUserId.Should().Be(sessionEnt.UserId);
            ex.Which.SessionAction.Should().Be(nameof(_sut.StopSessionAsync));
            await _botMessagesService.Received(1).SendErrorAsync(sessionEnt.UserId,
                "Session wasn't found. Use command /cancel and then /start_receiving to create a new one");
        }

        [Fact]
        public async Task GetSessionStateAsync_ShouldReturnAnException_WhenSessionEntityIsNotExists()
        {
            // Arrange
            var sessionEnt = FileReceiverSessionEntityFaker.GenerateFileReceivingSessionEntityWithRandomState();
            _receivingSessionRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(sessionEnt);

            // Act
            var state = await _sut.GetSessionStateAsync(sessionEnt.Id);

            // Assert
            state.Should().Be(sessionEnt.SessionState);
        }
    }
}
#pragma warning restore 1998
