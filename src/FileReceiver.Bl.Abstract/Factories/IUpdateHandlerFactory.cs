using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Common.Enums;

namespace FileReceiver.Bl.Abstract.Factories
{
    public interface IUpdateHandlerFactory
    {
        IUpdateHandler CreateUpdateHandler(TransactionType transactionType);
    }
}
