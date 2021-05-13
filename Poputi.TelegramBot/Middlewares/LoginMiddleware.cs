﻿using Poputi.TelegramBot.Core;
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
        private static readonly Regex _nameRegex = new(@"^[a-zA-Zа-яА-Я ,.'-]+$");

        public LoginMiddleware(TelegramContext telegramContext, IMiddleware next)
        {
            _telegramContext = telegramContext;
            _next = next;
        }

        public async ValueTask InvokeAsync(UpdateContext updateContext)
        {
            if (updateContext.CancellationToken.IsCancellationRequested)
            {
                return;
            }
            if (_telegramContext.Users.ContainsKey(updateContext.Update.Message.From.Id))
            {
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
                await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), "Ключик есть, а ларец не поддался :(");
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
                await CreateNewPoputiUser(updateContext, session);
                _telegramContext.LoginSessions.TryRemove(session.TelegramId, out _);
                _telegramContext.Users.TryAdd(session.TelegramId, new TelegramUser(session));
            }
        }

        private async ValueTask CreateNewPoputiUser(UpdateContext updateContext, UserLoginSession session)
        {
            var client = new HttpClient();
            var content = JsonContent.Create(new
            {
                login = $"telegramuser;{updateContext.Update.Message.From.Id}",
                password = session.Password,
                lastName = session.LastName,
                firstMidName = session.FirstName
            });
            var response = await client.PostAsync("https://localhost:5001/api/users", content);
            if (response.IsSuccessStatusCode)
            {
                await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), $"Добро пожаловать {session.FirstName} {session.LastName}");
                return;
            }
            await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), "Что-то пошло не так :(");
        }

        private async ValueTask ProceedLastName(UpdateContext updateContext, UserLoginSession session)
        {
            var isMessage = updateContext.Update.Type == UpdateType.Message;
            var isMatch = _nameRegex.IsMatch(updateContext.Update.Message.Text);
            if (isMessage && isMatch)
            {
                session.LastName = updateContext.Update.Message.Text;
                return;
            }
            await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), "Не могу записать такое имя, попробуй немного иначе...");
            await PromptForLastName(updateContext);
        }

        private async ValueTask ProceedFirstNameAndPromptLastName(UpdateContext updateContext, UserLoginSession session)
        {
            var isMessage = updateContext.Update.Type == UpdateType.Message;
            var isMatch = _nameRegex.IsMatch(updateContext.Update.Message.Text);
            if (isMessage && isMatch)
            {
                session.FirstName = updateContext.Update.Message.Text;
                await PromptForLastName(updateContext);
                return;
            }
            await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), "Не могу записать такое имя, попробуй немного иначе...");
            await PromptFirstName(updateContext);
        }

        private async Task CreateLoginSessionAndPromptFirstName(UpdateContext updateContext)
        {
            UserLoginSession newSession = new UserLoginSession();
            _telegramContext.LoginSessions.AddOrUpdate(updateContext.Update.Message.From.Id, newSession, (id, session) => session);
            await PromptFirstName(updateContext);
        }

        private async ValueTask PromptFirstName(UpdateContext updateContext)
        {
            await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), "Регистрация\nВведите свое имя");
        }

        private async ValueTask PromptForLastName(UpdateContext updateContext)
        {
            await updateContext.TelegramBotClient.SendTextMessageAsync(new ChatId(updateContext.Update.Message.Chat.Id), "Регистрация\nВведите свою фамилию");
        }
    }
}