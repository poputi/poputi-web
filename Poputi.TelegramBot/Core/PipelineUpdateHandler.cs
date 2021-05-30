using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Interfaces;
using Poputi.DataAccess.Services;
using Poputi.Logic.Interfaces;
using Poputi.Logic.Services;
using Poputi.TelegramBot.Middlewares;
using Poputi.Web.Auth;
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
        public static TelegramBotClient Bot;
        internal static CancellationToken CancellationToken;

        private static void ConfigureServices(ServiceCollection services)
        {
            //services.AddDbContext<MainContext>(options =>
            //{
            //    //options.UseNpgsql("Host=127.0.0.1;Port=5434;Username=postgres;Password=1;Database=poputi",
            //    options.UseNpgsql("Host=127.0.0.1;Port=5433;Username=postgres;Password=1;Database=poputi",
            //        // Надстройка для работы с пространственными данными.
            //        npgsqlOptions => npgsqlOptions.UseNetTopologySuite(geographyAsDefault: true)
            //        );
            //});

            //services.AddScoped<IRoutesService, RoutesService>();

            services.AddDbContext<MainContext>(options =>
            {
                // Можно хранить в launchSettings.json в переменных среды под именем "ConnectionStrings:DefaultConnection" для локальной машины.
                options.UseNpgsql("Host=127.0.0.1;Port=5433;Username=postgres;Password=1;Database=poputi",
                    // Надстройка для работы с пространственными данными.
                    npgsqlOptions => npgsqlOptions.UseNetTopologySuite(geographyAsDefault: true)
                    );
            });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IDriverService, DriverService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IdentityGenerator>();
            services.AddTransient<IRoutesService, RoutesService>();

            // Сервисы работы с геометрией.
            services.AddSingleton(new NtsGeometryServices());
        }

        public PipelineUpdateHandler()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            CreateDbIfNotExists(_serviceProvider);
        }

        private static void CreateDbIfNotExists(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                try
                {
                    var context = scopedServices.GetRequiredService<MainContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    //var logger = scopedServices.GetRequiredService<ILogger<Program>>();
                    //logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Произошла ошибка");
                Console.WriteLine(exception);
                Bot.StartReceiving(this, CancellationToken);
            });
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var routesService = scope.ServiceProvider.GetRequiredService<IRoutesService>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                var updateContext = new UpdateContext(botClient, update, cancellationToken, _telegramContext);

                var nullMiddleware = new NullMiddleware();
                var keyboard = new KeyboardMiddleware(nullMiddleware);
                var fellowTraveller = new FellowTravellerMiddleware(keyboard, routesService, userService);
                var driver = new DriverMiddleware(fellowTraveller, routesService, userService);
                var cancel = new CancelCommandMiddleware(driver);
                var login = new LoginMiddleware(cancel, userService);
                await login.InvokeAsync(updateContext);
            }
           
        }
    }
}
