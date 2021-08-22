using System;
using System.Threading.Tasks;

using FileReceiver.Common.Enums;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Abstract.Services
{
    public interface IFileReceivingService
    {
        Task UpdateFileReceivingState(long userId, FileReceivingState newState);
        Task FinishReceivingTransaction(long userId);
        Task<bool> SaveDocument(long userId, Guid sessionId, Document document);
        Task<FileReceivingState> GetFileReceivingStateForUser(long userId);
        Task<bool> CheckIfSessionExists(Guid token);
    }
}
