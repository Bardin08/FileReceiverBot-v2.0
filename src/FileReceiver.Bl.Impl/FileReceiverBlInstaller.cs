using System.Collections.Generic;

using AutoMapper;

using FileReceiver.Bl.Abstract.Factories;
using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl.AutoMapper.Profiles;
using FileReceiver.Bl.Impl.Factories;
using FileReceiver.Bl.Impl.Handlers.CallbackQuery;
using FileReceiver.Bl.Impl.Handlers.Commands;
using FileReceiver.Bl.Impl.Handlers.TelegramUpdate;
using FileReceiver.Bl.Impl.Services;

using Microsoft.Extensions.DependencyInjection;

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
            services.AddCallbackQueryHandlers();
            services.AddFactories();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IUpdateHandlerService, UpdateHandlerService>();
            services.AddTransient<IBotMessagesService, BotMessagesService>();
            services.AddTransient<IUserRegistrationService, UserRegistrationService>();
            services.AddTransient<IFileReceivingSessionService, FileReceivingSessionService>();
            services.AddTransient<IFileReceivingService, FileReceivingService>();
        }

        private static void AddMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfiles(
                new List<Profile>()
                {
                    new RegistrationProfile(),
                    new TransactionsProfile(),
                    new FileReceivingSessionProfile(),
                }
            ));
        }

        private static void AddCommandHandlers(this IServiceCollection services)
        {
            services.AddTransient<StartCommandHandler, StartCommandHandler>();
            services.AddTransient<RegisterCommandHandler, RegisterCommandHandler>();
            services.AddTransient<CancelCommandHandler, CancelCommandHandler>();
            services.AddTransient<DefaultCommandHandler, DefaultCommandHandler>();
            services.AddTransient<ProfileCommandHandler, ProfileCommandHandler>();
            services.AddTransient<ProfileEditCommandHandler, ProfileEditCommandHandler>();
            services.AddTransient<StartReceivingCommandHandler, StartReceivingCommandHandler>();
            services.AddTransient<SendFileCommandHandler, SendFileCommandHandler>();
        }

        private static void AddUpdateHandlers(this IServiceCollection services)
        {
            services.AddTransient<RegistrationUpdateHandler, RegistrationUpdateHandler>();
            services.AddTransient<DefaultUpdateHandler, DefaultUpdateHandler>();
            services.AddTransient<EditProfileUpdateHandler, EditProfileUpdateHandler>();
            services.AddTransient<FileReceivingSessionCreatingUpdateHandler, FileReceivingSessionCreatingUpdateHandler>();
            services.AddTransient<FileSendingUpdateHandler, FileSendingUpdateHandler>();
        }

        private static void AddCallbackQueryHandlers(this IServiceCollection services)
        {
            services.AddTransient<EditProfileCallbackQueryHandler, EditProfileCallbackQueryHandler>();
            services.AddTransient<FileReceivingSessionCallbackQueryHandler, FileReceivingSessionCallbackQueryHandler>();
        }

        private static void AddFactories(this IServiceCollection services)
        {
            services.AddTransient<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddTransient<IUpdateHandlerFactory, UpdateHandlerFactory>();
            services.AddTransient<ICallbackQueryHandlerFactory, CallbackQueryHandlerFactory>();
        }
    }
}
