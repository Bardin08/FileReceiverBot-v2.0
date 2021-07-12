using System.Collections.Generic;

using AutoMapper;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.AutoMapper.Profiles;
using FileReceiver.Bl.Impl.Factories;
using FileReceiver.Bl.Impl.Handlers.Commands;
using FileReceiver.Bl.Impl.Handlers.TelegramUpdate;
using FileReceiver.Bl.Impl.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace FileReceiver.Bl.Impl
{
    public static class FileReceiverBlInstaller
    {
        public static void AddBl(this IServiceCollection services)
        {
            services.AddServices();
            services.AddMapperConfiguration();
            services.AddCommandHandlers();
            services.AddUpdateHandlers();
            services.AddFactories();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUpdateHandlerService, UpdateHandlerService>();
            services.AddTransient<IBotMessagesService, BotMessagesService>();
            services.AddTransient<ISystemClock, SystemClock>();
        }

        private static void AddMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfiles(
                new List<Profile>()
                {
                    new RegistrationProfile(),
                    new TransactionsProfile(),
                }
            ));
        }

        private static void AddCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<StartCommandHandler, StartCommandHandler>();
            services.AddTransient<RegisterCommandHandler, RegisterCommandHandler>();
            services.AddTransient<CancelCommandHandler, CancelCommandHandler>();
            services.AddTransient<DefaultCommandHandler, DefaultCommandHandler>();
        }

        private static void AddUpdateHandlers(this IServiceCollection services)
        {
            services.AddTransient<RegistrationUpdateHandler, RegistrationUpdateHandler>();
            services.AddTransient<DefaultUpdateHandler, DefaultUpdateHandler>();
        }

        private static void AddFactories(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddTransient<IUpdateHandlerFactory, UpdateHandlerFactory>();
        }
    }
}
