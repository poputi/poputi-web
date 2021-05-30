using Poputi.TelegramBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.TelegramBot.Middlewares
{
    public class CancelCommandMiddleware : IMiddleware
    {
        private readonly IMiddleware _next;

        public CancelCommandMiddleware(IMiddleware next)
        {
            _next = next;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if (updateContext.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (updateContext.Update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                await _next.InvokeAsync(updateContext);
                return;
            }
            if (updateContext.Update.Message.Text == "/cancel" || updateContext.Update.Message.Text == "Отмена")
            {
                updateContext.TelegramContext.DriverSessions.TryRemove(updateContext.Update.Message.From.Id, out _);
                updateContext.TelegramContext.FellowTravellerSessions.TryRemove(updateContext.Update.Message.From.Id, out _);
                updateContext.TelegramContext.LoginSessions.TryRemove(updateContext.Update.Message.From.Id, out _);
            }
            await _next.InvokeAsync(updateContext);
        }
    }
}
