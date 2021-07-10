using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Handlers;

namespace FileReceiver.Bl.Abstract.Factories
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler CreateCommandHandler(string command);
    }
}
