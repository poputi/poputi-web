namespace Poputi.TelegramBot.Core
{
    public class TelegramUser
    {
        public TelegramUser(UserLoginSession session)
        {
            Id = session.TelegramId;
            Login = session.Login;
            Password = session.Password;
        }

        public string Login { get; set; }
        public long Id { get; set; }
        public string Password { get; set; }
    }
}