using FileReceiver.Dal;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Npgsql;

using Serilog;

namespace FileReceiverBot.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build()
                    .TryMigrate()
                    .Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHost TryMigrate(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<FileReceiverDbContext>();
            dbContext!.Database.Migrate();
            // reload enum mappings
            var connection = (NpgsqlConnection)dbContext.Database.GetDbConnection();
            connection.Open();
            connection.ReloadTypes();

            return host;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSerilog();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
