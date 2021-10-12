using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CG.Web.MegaApiClient;

using FileReceiver.Integrations.Mega.Configuration;

using Microsoft.Extensions.Options;

using MegaOptions = CG.Web.MegaApiClient.Options;

namespace FileReceiver.Integrations.Mega.Impl
{
    public class MegaClient : Abstract.IMegaApiClient
    {
        private readonly IMegaApiClient _client;
        private readonly MegaClientOptions _config;

        public MegaClient(IOptions<MegaClientOptions> megaOptions)
        {
            var config = megaOptions.Value;

            var clientOptions = new MegaOptions(
                config.ApplicationKey,
                config.SynchronizeApiRequests,
                null, // no custom request handler requires (null is default value)
                config.BufferSize,
                config.ChunksPackSize,
                config.ReportProgressChunkSize);

            _client = new MegaApiClient(clientOptions);
            _config = megaOptions.Value;
        }

        public async Task<MegaActionResponse> UploadFile(Guid transactionId, string token, string fileName,
            MemoryStream fileAsStream)
        {
            var actionDetails = new ActionDetails()
            {
                ActionId = Guid.NewGuid(),
                ActionType = MegaActionType.UploadFile,
                ActionTimestamp = DateTimeOffset.UtcNow,
            };

            await _client.LoginAsync(_config.Login, _config.Password);

            var folder = (await _client.GetNodesAsync()).FirstOrDefault(x => x.Name == token);
            if (folder is null)
            {
                return MegaActionResponse.Fail(transactionId, token, "Suitable folder wasn't found", actionDetails);
            }

            var file = await _client.UploadAsync(fileAsStream, fileName, folder);
            if (file is null)
            {
                return MegaActionResponse.Fail(transactionId, token, "File wasn't uploaded", actionDetails);
            }

            await _client.LogoutAsync();

            return MegaActionResponse.Success(transactionId, token, actionDetails);
        }

        public async Task<MegaActionResponse> DownloadFolder(Guid transactionId, string token, string nodeLink)
        {
            var actionDetails = new ActionDetails()
            {
                ActionId = Guid.NewGuid(),
                ActionType = MegaActionType.DownloadFolder,
                ActionTimestamp = DateTimeOffset.UtcNow,
            };

            await _client.LoginAsync(_config.Login, _config.Password);

            var folderNodeInfo = await _client.GetNodeFromLinkAsync(new Uri(nodeLink));
            var folder = (await _client.GetNodesAsync()).FirstOrDefault(x => x.Id == folderNodeInfo.Id);
            if (folder is null)
            {
                return MegaActionResponse.Fail(transactionId, token, "Suitable folder wasn't found", actionDetails);
            }

            actionDetails.Data = await _client.DownloadAsync(folder);
            await _client.LogoutAsync();

            return MegaActionResponse.Success(transactionId, token, actionDetails);
        }

        public async Task<MegaActionResponse> CreateFolder(Guid transactionId, string token)
        {
            var actionDetails = new ActionDetails()
            {
                ActionId = Guid.NewGuid(),
                ActionType = MegaActionType.CreateFolder,
                ActionTimestamp = DateTimeOffset.UtcNow,
            };

            await _client.LoginAsync(_config.Login, _config.Password);

            var nodes = (await _client.GetNodesAsync()).ToList();
            var root = nodes.FirstOrDefault(x => x.Type == NodeType.Root);
            if (root is null)
            {
                throw new InvalidOperationException("No root node found");
            }

            if (nodes.FirstOrDefault(x => x.Name == token) is not null)
            {
                throw new InvalidOperationException("Folder with this name is already exists");
            }

            var folder = await _client.CreateFolderAsync(token, root);
            if (folder is null)
            {
                return MegaActionResponse.Fail(transactionId, token, "Suitable folder wasn't found", actionDetails);
            }

            actionDetails.NodeLink = (await _client.GetDownloadLinkAsync(folder)).ToString();
            await _client.LogoutAsync();

            return MegaActionResponse.Success(transactionId, token, actionDetails);
        }

        public async Task<MegaActionResponse> DeleteFolder(Guid transactionId, string token, string nodeLink)
        {
            var actionDetails = new ActionDetails()
            {
                ActionId = Guid.NewGuid(),
                ActionType = MegaActionType.DeleteFolder,
                ActionTimestamp = DateTimeOffset.UtcNow,
            };

            await _client.LoginAsync(_config.Login, _config.Password);

            var folderNodeInfo = await _client.GetNodeFromLinkAsync(new Uri(nodeLink));
            var folder = (await _client.GetNodesAsync()).FirstOrDefault(x => x.Id == folderNodeInfo.Id);
            if (folder is null)
            {
                return MegaActionResponse.Fail(transactionId, token, "Suitable folder wasn't found", actionDetails);
            }

            await _client.DeleteAsync(folder, false);
            await _client.LogoutAsync();

            return MegaActionResponse.Success(transactionId, token, actionDetails);
        }
    }
}
