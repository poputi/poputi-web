using Poputi.Logic.Interfaces;
using Poputi.TelegramBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Poputi.TelegramBot.Middlewares
{
    /// <summary>
    /// Отвечает за то, чтобы у нас был Identity пользователя.
    /// Сразу запрашивает имя пользователя и добавляет его в освновную базу.
    /// На этапе концепции.
    /// По идее удобнее было бы сделать Transient.
    /// </summary>
    public class LoginMiddleware : IMiddleware
    {
        private readonly TelegramContext _telegramContext;
        private readonly IMiddleware _next;
        private readonly IUserService _userService;
        private static readonly Regex _nameRegex = new(@"^[a-zA-Zа-яА-Я ,.'-]+$");

        public LoginMiddleware(TelegramContext telegramContext, IMiddleware next, IUserService userService)
        {
            _telegramContext = telegramContext;
            _next = next;
            _userService = userService;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if (updateContext.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (updateContext.Update.Type != UpdateType.Message)
            {
                return;
            }
            if (_telegramContext.Users.Contains(updateContext.Update.Message.From.Id))
            {
                await _next.InvokeAsync(updateContext);
                return;
            }
            var user = await _userService.GetUserAsync(updateContext.Update.Message.From.Id.ToString());
            if (user != null)
            {
                _telegramContext.Users.Add(updateContext.Update.Message.From.Id);
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, $"Привет, {user.FirstMidName}!");
                await _next.InvokeAsync(updateContext);
                return;
            }
            if (!_telegramContext.LoginSessions.ContainsKey(updateContext.Update.Message.From.Id))
            {
                await CreateLoginSessionAndPromptFirstName(updateContext);
                return;
            }
            if (!_telegramContext.LoginSessions.TryGetValue(updateContext.Update.Message.From.Id, out UserLoginSession session))
            {
                await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Ключик есть, а ларец не поддался :(");
                return;
            }
            if (session.FirstName is null)
            {
                await ProceedFirstNameAndPromptLastName(updateContext, session);
                return;
            }
            if (session.LastName is null)
            {
                await ProceedLastName(updateContext, session);
            }
            await _next.InvokeAsync(updateContext);
        }

        private async ValueTask CreateNewPoputiUser(UpdateContext updateContext, UserLoginSession session)
        {
            await _userService.PostUserAsync(session.FirstName, session.LastName, updateContext.Update.Message.From.Id.ToString(), password: "1");
            //var client = new HttpClient();
            //var content = JsonContent.Create(new
            //{
            //    login = updateContext.Update.Message.From.Id.ToString(),
            //    password = "1",
            //    lastName = session.LastName,
            //    firstMidName = session.FirstName
            //});
            //var response = await client.PostAsync("https://localhost:5001/api/users", content);
            //if (response.IsSuccessStatusCode)
            //{
            //    await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, $"Добро пожаловать {session.FirstName} {session.LastName}\nТеперь вы можете создать поездку или найти готовую.");
            //    return;
            //}
            //await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Что-то пошло не так :(");
        }

        private async ValueTask ProceedLastName(UpdateContext updateContext, UserLoginSession session)
        {
            var isMatch = _nameRegex.IsMatch(updateContext.Update.Message.Text);
            if (isMatch)
            {
                session.LastName = updateContext.Update.Message.Text;
                await CreateNewPoputiUser(updateContext, session);
                _telegramContext.LoginSessions.TryRemove(session.TelegramId, out _);
                _telegramContext.Users.Add(session.TelegramId);
                return;
            }
            await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Не могу записать такое имя, попробуй немного иначе...");
            await PromptForLastName(updateContext);
        }

        private async ValueTask ProceedFirstNameAndPromptLastName(UpdateContext updateContext, UserLoginSession session)
        {
            var isMatch = _nameRegex.IsMatch(updateContext.Update.Message.Text);
            if (isMatch)
            {
                session.FirstName = updateContext.Update.Message.Text;
                await PromptForLastName(updateContext);
                return;
            }
            await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Не могу записать такое имя, попробуй немного иначе...");
            await PromptFirstName(updateContext);
        }

        private async ValueTask CreateLoginSessionAndPromptFirstName(UpdateContext updateContext)
        {
            UserLoginSession newSession = new UserLoginSession();
            _telegramContext.LoginSessions.AddOrUpdate(updateContext.Update.Message.From.Id, newSession, (id, session) => session);
            await PromptFirstName(updateContext);
        }

        private async ValueTask PromptFirstName(UpdateContext updateContext)
        {
            await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Регистрация\nВведите свое имя", replyMarkup: new ForceReplyMarkup());
        }

        private async ValueTask PromptForLastName(UpdateContext updateContext)
        {
            await updateContext.TelegramBotClient.SendTextMessageAsync(updateContext.Update.Message.Chat.Id, "Регистрация\nВведите свою фамилию", replyMarkup: new ForceReplyMarkup());
        }
    }
}
