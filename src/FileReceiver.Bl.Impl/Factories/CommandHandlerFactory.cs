using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Impl.Handlers.Commands;

using Microsoft.Extensions.DependencyInjection;

namespace FileReceiver.Bl.Impl.Factories
{
    public class CommandHandlerFactory : ICommandHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICommandHandler CreateCommandHandler(string command)
        {
            return command switch
            {
                "/start" => _serviceProvider.GetService<StartCommandHandler>(),
                "/register" => _serviceProvider.GetService<RegisterCommandHandler>(),
                "/cancel" => _serviceProvider.GetService<CancelCommandHandler>(),
                "/abort" => _serviceProvider.GetService<CancelCommandHandler>(),
                "/profile" => _serviceProvider.GetService<ProfileCommandHandler>(),
                "/profile_edit" => _serviceProvider.GetService<ProfileEditCommandHandler>(),
                "/start_receiving" => _serviceProvider.GetService<StartReceivingCommandHandler>(),
                "/send_file" => _serviceProvider.GetService<SendFileCommandHandler>(),
                { } when command.StartsWith("/") => _serviceProvider.GetService<DefaultCommandHandler>(),
                _ => null
            };
        }
    }
}
