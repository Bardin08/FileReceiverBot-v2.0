using FileReceiver.Integrations.Mega.Abstract;
using FileReceiver.Integrations.Mega.Configuration;
using FileReceiver.Integrations.Mega.Impl;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileReceiver.Integrations.Mega
{
    public static class MegaApiClientDependenciesInstaller
    {
        public static void AddMega(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IMegaApiClient, MegaClient>();

            services.Configure<MegaClientOptions>(config.GetSection("MegaClient"));
        }
    }
}
