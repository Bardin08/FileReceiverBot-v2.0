using FileReceiver.Infrastructure.HostedServices.HostedServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot;

namespace FileReceiver.Infrastructure.Configuration
{
    public static class TgBotInstaller
    {
        public static void AddTgBot(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<ITelegramBotClient>(_ => new TelegramBotClient(config["BotSettings:Token"]));

            if (bool.Parse(config["BotSettings:UseLongPolling"]))
            {
                services.AddHostedService<LongPolingService>();
            }
        }
    }
}
