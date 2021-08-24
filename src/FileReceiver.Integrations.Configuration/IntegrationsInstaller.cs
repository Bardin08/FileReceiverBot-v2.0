using FileReceiver.Integrations.Mega;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileReceiver.Integrations.Configuration
{
    public static class IntegrationsInstaller
    {
        public static void AddIntegrations(this IServiceCollection services, IConfiguration config)
        {
            services.AddMega(config);
        }
    }
}
