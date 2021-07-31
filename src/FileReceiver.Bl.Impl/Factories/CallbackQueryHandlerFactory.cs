using System;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Handlers;
using FileReceiver.Bl.Impl.Handlers.CallbackQuery;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types;

namespace FileReceiver.Bl.Impl.Factories
{
    public class CallbackQueryHandlerFactory : ICallbackQueryHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CallbackQueryHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICallbackQueryHandler CreateCallbackHandler(CallbackQuery callbackQuery)
        {
            return callbackQuery switch
            {
                { Data: { } data } when data.StartsWith("profile-")
                    => _serviceProvider.GetService<EditProfileCallbackQueryHandler>(),
                { Data: { } data } when data.StartsWith("fr-session")
                    => _serviceProvider.GetService<FileReceivingSessionCallbackQueryHandler>(),
            };
        }
    }
}
