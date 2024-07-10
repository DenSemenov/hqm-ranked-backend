using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_backend.Services;
using hqm_ranked_services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

namespace hqm_ranked_services
{
    public class TelegramService : BackgroundService
    {
        private static TelegramBotClient _bot;
        private RankedDb _dbContext;
        public TelegramService(IServiceScopeFactory scopeFactory)
        {

            var scope = scopeFactory.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<RankedDb>();

            var settings = _dbContext.Settings.FirstOrDefault();
            if (settings != null && !String.IsNullOrEmpty(settings.TelegramBotToken))
            {
                _bot = new TelegramBotClient(settings.TelegramBotToken);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_bot != null)
            {
                _bot.StartReceiving(HandleUpdate, OnError);
            }

            return Task.CompletedTask;
        }

        async Task OnError(ITelegramBotClient bot, Exception ex, CancellationToken ct)
        {

        }

        async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Message is null) return;       
            if (update.Message.Text is null) return;
            
            var msg = update.Message;
            var text = String.Empty;
            var buttons = new List<KeyboardButton>();
            switch (msg.Text)
            {
                case "/start":
                    buttons.Add(KeyboardButton.WithWebApp("Login", new WebAppInfo
                    {
                        Url="https://hqmfun.space/login"
                    }));
                    text = "Log in before use";
                    break;
                default:
                    text = "Wrong action";
                    break;
            }

            var keyboard = new ReplyKeyboardMarkup(buttons);

            await bot.SendTextMessageAsync(msg.Chat, text, replyMarkup: keyboard);
        }
    }
}
