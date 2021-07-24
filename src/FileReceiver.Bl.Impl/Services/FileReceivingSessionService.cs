using System;
using System.Threading.Tasks;

using AutoMapper;

using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Exceptions;
using FileReceiver.Common.Models;
using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Entities;
using FileReceiver.Dal.Entities.Enums;

namespace FileReceiver.Bl.Impl.Services
{
    public class FileReceivingSessionService : IFileReceivingSessionService
    {
        private readonly IBotMessagesService _botMessagesService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFileReceivingSessionRepository _receivingSessionRepository;
        private readonly IMapper _mapper;

        public FileReceivingSessionService(
            IBotMessagesService botMessagesService,
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            IFileReceivingSessionRepository receivingSessionRepository,
            IMapper mapper)
        {
            _botMessagesService = botMessagesService;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _receivingSessionRepository = receivingSessionRepository;
            _mapper = mapper;
        }

        public async Task CreateFileReceivingSessionAsync(long userId)
        {
            if (!await _userRepository.CheckIfUserExists(userId))
            {
                throw new UserProfileNotFoundException(userId, nameof(CreateFileReceivingSessionAsync));
            }

            var session = new FileReceivingSessionModel
            {
                UserId = userId,
                Constrains = new ConstraintsModel(),
                SessionState = FileReceivingSessionState.FileSizeConstraintSet,
                MaxFiles = 50,
                CreateData = DateTimeOffset.UtcNow,
            };

            var sessionEntity = await _receivingSessionRepository.AddAsync(
                _mapper.Map<FileReceivingSessionEntity>(session));

            var transactionDataModel = new TransactionDataModel();
            transactionDataModel.AddDataPiece(TransactionDataParameter.FileReceivingSessionId,
                sessionEntity.Id);

            var transaction = new TransactionModel
            {
                UserId = userId,
                TransactionState = TransactionState.Active,
                TransactionType = TransactionType.FileReceivingSessionCreating,
                TransactionData = transactionDataModel,
            };

            await _transactionRepository.AddAsync(_mapper.Map<TransactionEntity>(transaction));
        }

        public async Task SetFileSizeConstraintAsync(Guid sessionId, int bytes = 1000000)
        {
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);

            var session = _mapper.Map<FileReceivingSessionModel>(sessionEntity);
            session.Constrains.AddConstraint(ConstraintType.FileSize, bytes.ToString());

            sessionEntity.Constrains = session.Constrains.ConstraintsAsJson;
            sessionEntity.SessionState = FileReceivingSessionStateDb.FileNameConstraintSet;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);
        }

        public async Task SetFileNameConstraintAsync(Guid sessionId, string regexPatterns)
        {
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);

            var session = _mapper.Map<FileReceivingSessionModel>(sessionEntity);
            session.Constrains.AddConstraint(ConstraintType.FileName, regexPatterns);

            sessionEntity.Constrains = session.Constrains.ConstraintsAsJson;
            sessionEntity.SessionState = FileReceivingSessionStateDb.FileExtensionConstraintSet;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);
        }

        public async Task SetFileExtensionConstraintAsync(Guid sessionId, string extensions)
        {
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);

            var session = _mapper.Map<FileReceivingSessionModel>(sessionEntity);
            session.Constrains.AddConstraint(ConstraintType.FileExtension, extensions);

            sessionEntity.Constrains = session.Constrains.ConstraintsAsJson;
            sessionEntity.SessionState = FileReceivingSessionStateDb.SessionMaxFilesConstraint;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);
        }

        public async Task SetSessionMaxFilesConstraintAsync(long userId, Guid sessionId, int amount = 50)
        {
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);
            sessionEntity.MaxFiles = amount;
            sessionEntity.SessionState = FileReceivingSessionStateDb.SetFilesStorage;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);
        }

        public async Task SetFilesStorageAsync(long userId, FileStorageType storageType)
        {
            var sessionId = await GetSessionIdAndThrowExceptionIfNotExists(userId, nameof(SetFilesStorageAsync));
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);
            sessionEntity.Storage = (FileStorageTypeDb)storageType;
            sessionEntity.SessionState = FileReceivingSessionStateDb.ActiveSession;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);
        }

        public async Task<string> ExecuteSessionAsync(long userId)
        {
            var sessionId = await GetSessionIdAndThrowExceptionIfNotExists(userId, nameof(ExecuteSessionAsync));
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);
            sessionEntity.SessionState = FileReceivingSessionStateDb.ActiveSession;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);

            return sessionEntity.Id.ToString();
        }

        public async Task StopSessionAsync(long userId)
        {
            var sessionId = await GetSessionIdAndThrowExceptionIfNotExists(userId, nameof(StopSessionAsync));
            var sessionEntity = await _receivingSessionRepository.GetByIdAsync(sessionId);
            if (sessionEntity.UserId != userId)
            {
                await _botMessagesService.SendErrorAsync(userId, "You can stop only yours sessions");
                return;
            }
            sessionEntity.SessionState = FileReceivingSessionStateDb.EndedSession;
            sessionEntity.SessionEndReason = SessionEndReasonDb.EndedByOwner;
            await _receivingSessionRepository.UpdateAsync(sessionEntity);
            await _botMessagesService.SendErrorAsync(userId, "Session stopped");
        }

        public async Task<FileReceivingSessionState> GetSessionStateAsync(Guid sessionId)
        {
            return (FileReceivingSessionState)(await _receivingSessionRepository.GetByIdAsync(sessionId)).SessionState;
        }

        public async Task<Guid?> GetFirstActiveFileReceivingSessionIdByUserIdAsync(long userId)
        {
            var transactionEntity = await _transactionRepository
                .GetByUserIdAsync(userId, TransactionTypeDb.FileReceivingSessionCreating);
            var transactionData = new TransactionDataModel(transactionEntity.TransactionData);
            return new Guid((string)transactionData.GetDataPiece(TransactionDataParameter.FileReceivingSessionId));
        }

        private async Task<Guid> GetSessionIdAndThrowExceptionIfNotExists(long userId, string actionName)
        {
            var sessionId = await GetFirstActiveFileReceivingSessionIdByUserIdAsync(userId);
            if (sessionId.HasValue)
            {
                return sessionId.Value;
            }

            await _botMessagesService.SendErrorAsync(userId,
                "Session wasn't found. Use command /cancel and then /start_receiving to create a new one");
            throw new FileReceivingSessionActionErrorException(userId, actionName);
        }
    }
}
