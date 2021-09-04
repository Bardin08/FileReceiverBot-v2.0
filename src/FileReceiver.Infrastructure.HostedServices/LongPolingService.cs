using System;
using System.Threading;
using System.Threading.Tasks;

using FileReceiver.Bl.Abstract.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace FileReceiver.Infrastructure.HostedServices
{
    public class LongPolingService : BackgroundService
    {
        private readonly QueuedUpdateReceiver _updateReceiver;
        private readonly IServiceProvider _serviceProvider;

        public LongPolingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _updateReceiver = new QueuedUpdateReceiver(serviceProvider.GetService<ITelegramBotClient>()!);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            _updateReceiver.StartReceiving(cancellationToken: stoppingToken);

            await foreach (Update update in _updateReceiver.YieldUpdatesAsync().WithCancellation(stoppingToken))
            {
                try
                {
                    var updateHandlerService =
                        _serviceProvider.CreateScope().ServiceProvider.GetService<IUpdateHandlerService>();
                    if (updateHandlerService != null)
                    {
                        await updateHandlerService.HandleUpdateAsync(update);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
