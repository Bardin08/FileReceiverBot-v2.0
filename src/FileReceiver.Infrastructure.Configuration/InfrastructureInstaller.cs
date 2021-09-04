using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileReceiver.Infrastructure.Configuration
{
    public static class InfrastructureInstaller
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddTgBot(config);
            services.AddSwagger();
            SerilogInstaller.AddSerilogLogging(config);
        }
    }
}
