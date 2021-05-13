using Poputi.TelegramBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.TelegramBot.Middlewares
{
    public class NullMiddleware : IMiddleware
    {
        public ValueTask InvokeAsync(UpdateContext updateContext)
        {
            return ValueTask.CompletedTask;
        }
    }
}
