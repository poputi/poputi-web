using Poputi.TelegramBot.Core;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Poputi.TelegramBot.Middlewares
{
    internal class KeyboardMiddleware : IMiddleware
    {
        private readonly IMiddleware _next;

        public KeyboardMiddleware(IMiddleware next)
        {
            _next = next;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if (updateContext.IsResponsed)
            {
                return;
            }
            var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[] { "Создать поездку", "Найти поездку", "Отмена" }, resizeKeyboard: true);
            await updateContext.TelegramBotClient.SendTextMessageAsync(
                chatId: updateContext.Update.Message.Chat.Id,
                text: "Навигация",
                replyMarkup: keyboard
            );
            await _next.InvokeAsync(updateContext);
        }
    }
}