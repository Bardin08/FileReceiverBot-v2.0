using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Impl.Handlers.TelegramUpdate;
using FileReceiver.Common.Enums;
using FileReceiver.Common.Models;

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

        public IUpdateHandler CreateUpdateHandler(TransactionModel transaction)
        {
            return transaction.TransactionType switch
            {
                TransactionType.Registration => _serviceProvider.GetService<RegistrationUpdateHandler>(),
                TransactionType.Unknown => _serviceProvider.GetService<DefaultUpdateHandler>(),
                _ => _serviceProvider.GetService<DefaultUpdateHandler>(),
            };
        }
    }
}
