using System;

namespace Poputi.TelegramBot.Core
{
    public class UserLoginSession
    {
        public long TelegramId { get; set; }
        public string Login => $"telegramuser;{TelegramId}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; } = Guid.NewGuid().ToString();
        public string PhoneNumber { get; internal set; }
    }
}