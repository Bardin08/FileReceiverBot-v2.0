using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Common.Models;

namespace FileReceiver.Bl.Abstract.Factories
{
    public interface IUpdateHandlerFactory
    {
        Task<IUpdateHandler> CreateUpdateHandlerAsync(TransactionModel transaction);
    }
}
