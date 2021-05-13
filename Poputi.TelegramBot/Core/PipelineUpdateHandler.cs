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
        public UpdateType[] AllowedUpdates => (UpdateType[])Enum.GetValues(typeof(UpdateType));

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
            var updateContext = new UpdateContext(botClient, update, cancellationToken);
            var login = new LoginMiddleware(_telegramContext, new NullMiddleware());
            await login.InvokeAsync(updateContext);
        }
    }
}
