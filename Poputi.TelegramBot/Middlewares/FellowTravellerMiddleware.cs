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
        private TelegramContext _telegramContext;
        private IMiddleware _next;
        private IGeocodingService _geocodingService;
        private IRoutesService _routesService;
        public FellowTravellerMiddleware(TelegramContext telegramContext, IMiddleware next, IRoutesService routesService)
        {
            _telegramContext = telegramContext;
            _next = next;
            _geocodingService = new YandexGeocoding();
            _routesService = routesService;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if (updateContext.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (updateContext.Update.Type != UpdateType.Message)
            {
                await _next.InvokeAsync(updateContext);
                return;
            }

            if (!_telegramContext.FellowTravellerSession.ContainsKey(updateContext.Update.Message.From.Id))
            {
                var newSession = new FellowTravellerSession();
                _telegramContext.FellowTravellerSession.AddOrUpdate(updateContext.Update.Message.From.Id, newSession, (id, session) => session);

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите стартовый адресс", replyMarkup: new ForceReplyMarkup());
                return;
            }

            if (!_telegramContext.FellowTravellerSession.TryGetValue(updateContext.Update.Message.Chat.Id, out FellowTravellerSession fellowTravellerSession))
            {
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Ключик есть, а ларец не поддался :(");
                return;
            }

            if (fellowTravellerSession.Start is null)
            {
                var isMessage = updateContext.Update.Type == UpdateType.Message;
                (var error, var point) = await _geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (isMessage && error is null)
                {
                    fellowTravellerSession.Start = point;
                    await updateContext.TelegramBotClient.SendVenueAsync(
                        chatId: updateContext.Update.Message.Chat.Id,
                        latitude: (float)point.Coordinate.Y,
                        longitude: (float)point.Coordinate.X,
                        title: "Стартовая точка",
                        address: updateContext.Update.Message.Text);

                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите конечный адресс", replyMarkup: new ForceReplyMarkup());
                    return;
                }
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, error);
                return;
            }

            if (fellowTravellerSession.End is null)
            {
                var isMessage = updateContext.Update.Type == UpdateType.Message;
                (var error, var point) = await _geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (isMessage && error is null)
                {
                    fellowTravellerSession.End = point;
                    await updateContext.TelegramBotClient.SendVenueAsync(
                       chatId: updateContext.Update.Message.Chat.Id,
                       latitude: (float)point.Coordinate.Y,
                       longitude: (float)point.Coordinate.X,
                       title: "Конечная точка",
                       address: updateContext.Update.Message.Text);
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите дату и время" + DateTime.Now, replyMarkup: new ForceReplyMarkup());
                    return;
                }

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, error);

            }

            if (fellowTravellerSession.DateTime is null)
            {
                var isMessage = updateContext.Update.Type == UpdateType.Message;
                var isDateTimeValid = DateTime.TryParse(updateContext.Update.Message.Text, out var dateTime);
                var validateError = "Время не валидно";
                if (isMessage && isDateTimeValid)
                {
                    fellowTravellerSession.DateTime = dateTime.ToString();

                    var cityRoute = new CityRoute
                    {
                        CityRouteType = DataAccess.Enums.CityRouteType.ByFellowTraveler,
                        End = fellowTravellerSession.End,
                        Start = fellowTravellerSession.Start,
                        DateTime = dateTime,
                        UserId = 1,//TO DO: сконектить юзера бота и приложения 
                    };
                    var routes = await _routesService.FindNotMatchedRoutesWithinAsync(cityRoute, 500).ToListAsync();
                    await _routesService.AddTravellerRouteAsync(cityRoute);
                    if (routes.Count > 1)
                    {
                        var buttons = new KeyboardButton[routes.Count];
                        for (var i = 0; i < routes.Count; i++)
                        {
                            buttons[i] = $"Фамилия: {routes[i].User.FirstMidName}\n Имя: {routes[i].User.LastName}";
                        }
                        var keyboard = new ReplyKeyboardMarkup(buttons, oneTimeKeyboard: true);
                        await updateContext.TelegramBotClient.SendTextMessageAsync(
                            chatId: updateContext.Update.Message.Chat.Id,
                            text: "Выберите водителя?",
                            replyMarkup: keyboard);
                    }
                    _telegramContext.FellowTravellerSession.TryRemove(updateContext.Update.Message.Chat.Id, out _);
                    return;
                }

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, validateError);

            }
            await _next.InvokeAsync(updateContext);
        }
    }
}
