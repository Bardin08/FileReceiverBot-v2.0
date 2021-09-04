using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FileReceiver.Infrastructure.Configuration
{
    public static class SwaggerInstaller
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileReceiverBot.Api", Version = "v1" });
            });
        }
    }
}
