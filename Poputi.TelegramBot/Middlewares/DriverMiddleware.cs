using Poputi.TelegramBot.Core;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace Poputi.TelegramBot.Middlewares
{
    internal class DriverMiddleware : IMiddleware
    {
        private TelegramContext _telegramContext;
        private IMiddleware _next;

        public DriverMiddleware(TelegramContext telegramContext, IMiddleware next)
        {
            _telegramContext = telegramContext;
            _next = next;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if (updateContext.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (updateContext.Update.Type != UpdateType.Message || updateContext.Update.Message.Text != "/poezdka" && updateContext.Update.Message.Text != "Создать поездку")
            {
                await _next.InvokeAsync(updateContext);
                return;
            }
            // АПИ ТОКЕНА ТО НЕТ!...
            await _next.InvokeAsync(updateContext);
        }
    }
}