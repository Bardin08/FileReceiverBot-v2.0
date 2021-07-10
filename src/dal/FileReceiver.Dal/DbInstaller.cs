using FileReceiver.Dal.Abstract.Repositories;
using FileReceiver.Dal.Impl;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileReceiver.Dal
{
    public static class DbInstaller
    {
        public static void AddDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FileReceiverDbContext>(x =>
                x.UseNpgsql(configuration["ConnectionStrings:FileReceiverDb"]));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
        }
    }
}
