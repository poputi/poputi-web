using Poputi.TelegramBot.Core;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Poputi.Logic;
using Poputi.TelegramBot.Sessions;
using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace Poputi.TelegramBot.Middlewares
{
    public class FellowTravellerMiddleware : IMiddleware
    {
        private TelegramContext _telegramContext;
        private IMiddleware _next;
        private IGeocodingService geocodingService;
        public FellowTravellerMiddleware(TelegramContext telegramContext, IMiddleware next)
        {
            _telegramContext = telegramContext;
            _next = next;
            geocodingService = new YandexGeocoding();
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

            if(!_telegramContext.FellowTravellerSession.TryGetValue(updateContext.Update.Message.Chat.Id, out FellowTravellerSession fellowTravellerSession))
            {
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Ключик есть, а ларец не поддался :(");
                return;
            }

            if(fellowTravellerSession.Start is null)
            {
                var isMessage = updateContext.Update.Type == UpdateType.Message;
                (var error, var point) = await geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if(isMessage && error is null)
                {
                    fellowTravellerSession.Start = point;
                    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Введите конечный адресс", replyMarkup: new ForceReplyMarkup());
                    return;
                }
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, error);
                return;
            }

            if (fellowTravellerSession.End is null)
            {
                var isMessage = updateContext.Update.Type == UpdateType.Message;
                (var error, var point) = await geocodingService.GetGeocode(updateContext.Update.Message.Text);
                if (isMessage && error is null)
                {
                    fellowTravellerSession.End = point;
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
                    _telegramContext.FellowTravellerSession.TryRemove(updateContext.Update.Message.Chat.Id, out _);
                    //TODO: сохранить поиск поездки / вернуть список поездок на выбор
                    return;
                }

                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, validateError);

            }
            await _next.InvokeAsync(updateContext);
        }
    }
}
