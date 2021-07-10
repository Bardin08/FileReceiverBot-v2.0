using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Common.Models;

namespace FileReceiver.Bl.Impl.Factories
{
    public class UpdateHandlerFactory : IUpdateHandlerFactory
    {
        public Task<IUpdateHandler> CreateUpdateHandlerAsync(TransactionModel transaction)
        {
            throw new System.NotImplementedException();
        }
    }
}
