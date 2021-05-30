using Poputi.DataAccess.Daos;
using Poputi.Logic;
using Poputi.Logic.Interfaces;
using Poputi.TelegramBot.Core;
using Poputi.TelegramBot.Sessions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Poputi.TelegramBot.Middlewares
{
    internal class DriverMiddleware : IMiddleware
    {
        private readonly IMiddleware _next;
        private readonly IRoutesService _routesService;
        private readonly IUserService _userService;
        private readonly YandexGeocoding _geocodingService;

        public DriverMiddleware(IMiddleware next, IRoutesService routesService, IUserService userService)
        {
            _next = next;
            _routesService = routesService;
            _userService = userService;
            _geocodingService = new YandexGeocoding();
        }


        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            var telegramContext = updateContext.TelegramContext;
            if (updateContext.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (updateContext.Update.Type != UpdateType.Message)
            {
                await _next.InvokeAsync(updateContext);
                return;
            }
            // Если нет сессии и команда не наша, то ливаем.
            if (!telegramContext.DriverSessions.ContainsKey(updateContext.Update.Message.From.Id) && updateContext.Update.Message.Text != "/craete" && updateContext.Update.Message.Text != "Создать поездку")
            {
                await _next.InvokeAsync(updateContext);
                return;
            }
            if (!telegramContext.DriverSessions.ContainsKey(updateContext.Update.Message.From.Id))
            {
                var newSession = new FellowTravellerSession();
                telegramContext.DriverSessions.TryAdd(updateContext.Update.Message.From.Id, newSession);

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите стартовый адрес");
                return;
            }

            if (!telegramContext.DriverSessions.TryGetValue(updateContext.Update.Message.From.Id, out FellowTravellerSession driverSession))
            {
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Ключик есть, а ларец не поддался :(");
                return;
            }

            if (driverSession.Start is null)
            {
                (var error, var point) = await _geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (error is null)
                {
                    driverSession.Start = point;
                    await updateContext.TelegramBotClient.SendVenueAsync(
                        chatId: updateContext.Update.Message.Chat.Id,
                        latitude: (float)point.Coordinate.Y,
                        longitude: (float)point.Coordinate.X,
                        title: "Стартовая точка",
                        address: updateContext.Update.Message.Text);

                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите конечный адрес");
                }
                else
                {
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Прости, не разобрал адрес, попробуй написать точнее...");
                }
                return;
            }

            if (driverSession.End is null)
            {
                (var error, var point) = await _geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (error is null)
                {
                    driverSession.End = point;
                    await updateContext.TelegramBotClient.SendVenueAsync(
                       chatId: updateContext.Update.Message.Chat.Id,
                       latitude: (float)point.Coordinate.Y,
                       longitude: (float)point.Coordinate.X,
                       title: "Конечная точка",
                       address: updateContext.Update.Message.Text);
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите дату и время. Пример: " + DateTime.Now);
                }
                else
                {
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Прости, не разобрал адрес, попробуй написать точнее...");
                }
                return;
            }

            if (driverSession.DateTime is null)
            {
                if (!DateTime.TryParse(updateContext.Update.Message.Text, out var dateTime))
                {
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Прости, не разобрал время, попробуй написать точнее...");
                }
                driverSession.DateTime = dateTime.ToString();
                var user = await _userService.GetUserAsync(updateContext.Update.Message.From.Id.ToString());
                var userId = user.Id;
                var cityRoute = new CityRoute
                {
                    CityRouteType = DataAccess.Enums.CityRouteType.ByDriver,
                    End = driverSession.End,
                    Start = driverSession.Start,
                    DateTime = dateTime,
                    UserId = userId,
                };
                await _routesService.AddDriverRouteAsync(cityRoute);

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Готово! Мы напишем, когда найдем попутчика.");
            }
            telegramContext.DriverSessions.TryRemove(updateContext.Update.Message.From.Id, out _);
            await _next.InvokeAsync(updateContext);
        }
    }
}