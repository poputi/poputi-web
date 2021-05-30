using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Poputi.TelegramBot.Core
{
    public class UpdateContext
    {
        public UpdateContext(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken, TelegramContext telegramContext)
        {
            CancellationToken = cancellationToken;
            TelegramBotClient = telegramBotClient;
            Update = update;
            TelegramContext = telegramContext;
        }

        public CancellationToken CancellationToken { get; }
        public ITelegramBotClient TelegramBotClient { get; }
        public Update Update { get; }
        public bool IsResponsed { get; set; }
        public TelegramContext TelegramContext { get; }
    }
}
