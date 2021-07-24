using System;
using System.Threading.Tasks;

using FileReceiver.Common.Enums;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IFileReceivingSessionService
    {
        Task CreateFileReceivingSessionAsync(long userId);
        Task SetFileSizeConstraintAsync(Guid sessionId, int bytes = 1_000_000);
        Task SetFileNameConstraintAsync(Guid sessionId, string regexPatterns);
        Task SetFileExtensionConstraintAsync(Guid sessionId, string extensions);
        Task SetSessionMaxFilesConstraintAsync(long userId, Guid sessionId, int amount = 50);
        Task SetFilesStorageAsync(long userId, FileStorageType storageType);
        Task<string> ExecuteSessionAsync(long userId);
        Task StopSessionAsync(long userId);
        Task<FileReceivingSessionState> GetSessionStateAsync(Guid sessionId);
        Task<Guid?> GetFirstActiveFileReceivingSessionIdByUserIdAsync(long userId);
    }
}
