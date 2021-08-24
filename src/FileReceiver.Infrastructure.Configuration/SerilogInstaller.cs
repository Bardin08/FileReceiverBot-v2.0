using Microsoft.Extensions.Configuration;

using Serilog;

namespace FileReceiver.Infrastructure.Configuration
{
    public static class SerilogInstaller
    {
        public static void AddSerilogLogging(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
