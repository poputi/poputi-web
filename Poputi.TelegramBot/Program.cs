using Poputi.TelegramBot.Core;
using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace Poputi.TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBotClient("1715566619:AAGhlh19WpjguPmSNc2hKdRHvbwlHqH3_9U");
            var cts = new CancellationTokenSource();
            bot.StartReceiving<PipelineUpdateHandler>(cts.Token);
            PipelineUpdateHandler.Bot = bot;
            PipelineUpdateHandler.CancellationToken = cts.Token;

            Console.WriteLine($"Start listening");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}
