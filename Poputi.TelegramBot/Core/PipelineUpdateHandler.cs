using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Poputi.DataAccess.Contexts;
using Poputi.Logic.Interfaces;
using Poputi.Logic.Services;
using Poputi.TelegramBot.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Poputi.TelegramBot.Core
{
    public class PipelineUpdateHandler : IUpdateHandler
    {
        private TelegramContext _telegramContext = new TelegramContext();
        private IServiceProvider _serviceProvider;
        public UpdateType[] AllowedUpdates => (UpdateType[])Enum.GetValues(typeof(UpdateType));

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<MainContext>(options =>
            {
                options.UseNpgsql("Host=127.0.0.1;Port=5434;Username=postgres;Password=1;Database=poputi",
                    // Надстройка для работы с пространственными данными.
                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite(geographyAsDefault: true)
                    );
            });

            services.AddScoped<IRoutesService, RoutesService>();
        }

        public PipelineUpdateHandler()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Произошла ошибка");
                Console.WriteLine(exception);
            });
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var routesService = scope.ServiceProvider.GetRequiredService<IRoutesService>(); ;

                var updateContext = new UpdateContext(botClient, update, cancellationToken);
                IMiddleware nullMiddleware = new NullMiddleware();
                IMiddleware fellowTraveller = new FellowTravellerMiddleware(_telegramContext, nullMiddleware, routesService);
                IMiddleware keyboard = new KeyboardMiddleware(_telegramContext, fellowTraveller);
                IMiddleware driver = new DriverMiddleware(_telegramContext, keyboard);
                var login = new LoginMiddleware(_telegramContext, driver);
                await login.InvokeAsync(updateContext);
            }
           
        }
    }
}
