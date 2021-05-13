using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Poputi.TelegramBot.Core
{
    public class UpdateContext
    {
        public UpdateContext(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            TelegramBotClient = telegramBotClient;
            Update = update;
        }

        public CancellationToken CancellationToken { get; }
        public ITelegramBotClient TelegramBotClient { get; }
        public Update Update { get; }
    }
}
