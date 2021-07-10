using FileReceiver.Bl.Abstract.Services;
using FileReceiver.Bl.Impl;
using FileReceiver.Bl.Impl.Services;
using FileReceiver.Dal;

using FileReceiverBot.Api.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Telegram.Bot;

namespace FileReceiverBot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddTransient<IUpdateHandlerService, UpdateHandlerService>();

            var botSettings = new BotSettings();
            Configuration.Bind(nameof(BotSettings), botSettings);

            services.AddSingleton(botSettings);
            services.AddTransient<ITelegramBotClient>(o => new TelegramBotClient(Configuration["BotSettings:Token"]));
            services.AddBl();
            services.AddDb(Configuration);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileReceiverBot.Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileReceiverBot.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
