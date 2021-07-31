using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Impl.Handlers.TelegramUpdate;
using FileReceiver.Common.Enums;

using Microsoft.Extensions.DependencyInjection;

namespace FileReceiver.Bl.Impl.Factories
{
    public class UpdateHandlerFactory : IUpdateHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUpdateHandler CreateUpdateHandler(TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.Unknown => _serviceProvider.GetService<DefaultUpdateHandler>(),
                TransactionType.Registration => _serviceProvider.GetService<RegistrationUpdateHandler>(),
                TransactionType.EditProfile => _serviceProvider.GetService<EditProfileUpdateHandler>(),
                TransactionType.FileReceivingSessionCreating => _serviceProvider.GetService<FileReceivingSessionCreatingUpdateHandler>(),
                _ => _serviceProvider.GetService<DefaultUpdateHandler>(),
            };
        }
    }
}
