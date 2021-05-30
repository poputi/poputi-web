using Poputi.TelegramBot.Core;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Poputi.Logic;
using Poputi.TelegramBot.Sessions;
using System;
using Telegram.Bot.Types.ReplyMarkups;
using Poputi.Logic.Interfaces;
using Poputi.DataAccess.Daos;
using System.Linq;
using System.Collections.Generic;

namespace Poputi.TelegramBot.Middlewares
{
    public class FellowTravellerMiddleware : IMiddleware
    {
        private IMiddleware _next;
        private IGeocodingService _geocodingService;
        private IRoutesService _routesService;
        private readonly IUserService _userService;

        public FellowTravellerMiddleware(IMiddleware next, IRoutesService routesService, IUserService userService)
        {
            _next = next;
            _geocodingService = new YandexGeocoding();
            _routesService = routesService;
            _userService = userService;
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
            if (!telegramContext.FellowTravellerSessions.ContainsKey(updateContext.Update.Message.From.Id) && updateContext.Update.Message.Text != "/poisk" && updateContext.Update.Message.Text != "Найти поездку")
            {
                await _next.InvokeAsync(updateContext);
                return;
            }
            if (!telegramContext.FellowTravellerSessions.ContainsKey(updateContext.Update.Message.From.Id))
            {
                var newSession = new FellowTravellerSession();
                telegramContext.FellowTravellerSessions.TryAdd(updateContext.Update.Message.From.Id, newSession);

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите стартовый адрес");
                return;
            }

            if (!telegramContext.FellowTravellerSessions.TryGetValue(updateContext.Update.Message.From.Id, out FellowTravellerSession fellowTravellerSession))
            {
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Ключик есть, а ларец не поддался :(");
                return;
            }

            if (fellowTravellerSession.Start is null)
            {
                (var error, var point) = await _geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (error is null)
                {
                    fellowTravellerSession.Start = point;
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

            if (fellowTravellerSession.End is null)
            {
                (var error, var point) = await _geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (error is null)
                {
                    fellowTravellerSession.End = point;
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
            var user = await _userService.GetUserAsync(updateContext.Update.Message.From.Id.ToString());
            if (fellowTravellerSession.DateTime is null)
            {
                if (!DateTime.TryParse(updateContext.Update.Message.Text, out var dateTime))
                {
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Прости, не разобрал время, попробуй написать точнее...");
                }
                fellowTravellerSession.DateTime = dateTime.ToString();
                var userId = user.Id;
                var cityRoute = new CityRoute
                {
                    CityRouteType = DataAccess.Enums.CityRouteType.ByFellowTraveler,
                    End = fellowTravellerSession.End,
                    Start = fellowTravellerSession.Start,
                    DateTime = dateTime,
                    UserId = userId,
                };
                var routes = await _routesService.FindNotMatchedRoutesWithinAsync(cityRoute, 500).ToListAsync();
                //await _routesService.AddTravellerRouteAsync(cityRoute);
                if (routes.Count > 0)
                {
                    var buttons = new KeyboardButton[routes.Count];
                    for (var i = 0; i < routes.Count; i++)
                    {
                        var option = $"Фамилия: {routes[i].User.LastName}\n Имя: {routes[i].User.FirstMidName}";
                        buttons[i] = option;
                        fellowTravellerSession.RoutesOptions[option] = routes[i];
                    }
                    var keyboard = new ReplyKeyboardMarkup(buttons, oneTimeKeyboard: true);
                    await updateContext.TelegramBotClient.SendTextMessageAsync(
                        chatId: updateContext.Update.Message.Chat.Id,
                        text: "Выберите водителя или /cancel",
                        replyMarkup: keyboard);
                    updateContext.IsResponsed = true;
                }
                else
                {
                    telegramContext.FellowTravellerSessions.TryRemove(updateContext.Update.Message.From.Id, out _);
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Прости, подходящих поездок пока что нет :(");
                }
                return;

            }

            if (fellowTravellerSession.RoutesOptions.TryGetValue(updateContext.Update.Message.Text, out CityRoute chosenRoute))
            {
                await _routesService.MatchRoutesAsync(chosenRoute);
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Отлично! Телефон водителя:");
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, chosenRoute.User.PhoneNumber);

                await updateContext.TelegramBotClient.SendTextMessageAsync(long.Parse(chosenRoute.User.Login), $"Найден попутчик!\n{user.FirstMidName} {user.LastName}\nТелефон:");
                await updateContext.TelegramBotClient.SendTextMessageAsync(long.Parse(chosenRoute.User.Login), user.PhoneNumber);
            }
            else
            {
                var count = fellowTravellerSession.RoutesOptions.Count;
                var buttons = new KeyboardButton[count];
                for (var i = 0; i < count; i++)
                {
                    buttons[i] = fellowTravellerSession.RoutesOptions.ElementAt(i).Key;
                }
                var keyboard = new ReplyKeyboardMarkup(buttons, oneTimeKeyboard: true);
                await updateContext.TelegramBotClient.SendTextMessageAsync(
                    chatId: updateContext.Update.Message.Chat.Id,
                    text: "Пожалуйста, выберите водителя из списка или /cancel",
                    replyMarkup: keyboard);
                updateContext.IsResponsed = true;
                return;
            }
            telegramContext.FellowTravellerSessions.TryRemove(updateContext.Update.Message.From.Id, out _);
            await _next.InvokeAsync(updateContext);
        }
    }
}
