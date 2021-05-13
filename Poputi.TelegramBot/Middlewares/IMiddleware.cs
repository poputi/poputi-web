using Poputi.TelegramBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.TelegramBot.Middlewares
{
    public interface IMiddleware
    {
        ValueTask InvokeAsync(UpdateContext updateContext);
    }
}
