using Poputi.TelegramBot.Core;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Poputi.TelegramBot.Middlewares
{
    internal class KeyboardMiddleware : IMiddleware
    {
        private readonly TelegramContext _telegramContext;
        private readonly IMiddleware _next;

        public KeyboardMiddleware(TelegramContext telegramContext, IMiddleware next)
        {
            _telegramContext = telegramContext;
            _next = next;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if(!_telegramContext.FellowTravellerSession.ContainsKey(updateContext.Update.Message.Chat.Id))
            {
                var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[] { "Создать поездку", "Найти поездку" }, resizeKeyboard: true);
                await updateContext.TelegramBotClient.SendTextMessageAsync(
                    chatId: updateContext.Update.Message.Chat.Id,
                    text: "Кому попути?",
                    replyMarkup: keyboard
                );
            }     
            await _next.InvokeAsync(updateContext);
        }
    }
}