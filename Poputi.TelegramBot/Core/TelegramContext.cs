using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.TelegramBot.Core
{
    public class TelegramContext
    {
        public ConcurrentDictionary<long, UserLoginSession> LoginSessions { get; } = new ConcurrentDictionary<long, UserLoginSession>();
        public ConcurrentDictionary<long, TelegramUser> Users { get; } = new ConcurrentDictionary<long, TelegramUser>();
    }
}
