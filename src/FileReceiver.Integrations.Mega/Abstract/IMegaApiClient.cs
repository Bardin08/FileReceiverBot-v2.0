using System;
using System.IO;
using System.Threading.Tasks;

namespace FileReceiver.Integrations.Mega.Abstract
{
    public interface IMegaApiClient
    {
        Task<MegaActionResponse> UploadFile(Guid transactionId, string token, string nodeLink,
            MemoryStream fileAsStream);
        Task<MegaActionResponse> DownloadFolder(Guid transactionId, string token, string nodeLink);
        Task<MegaActionResponse> CreateFolder(Guid transactionId, string token);
        Task<MegaActionResponse> DeleteFolder(Guid transactionId, string token, string nodeLink);
    }
}
