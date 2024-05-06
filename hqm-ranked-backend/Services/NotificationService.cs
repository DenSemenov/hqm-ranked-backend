using CSharpDiscordWebhook.NET.Discord;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class NotificationService : INotificationService
    {
        private RankedDb _dbContext;
        public NotificationService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendDiscordNotification(string serverName, int count, int teamMax)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNotificationWebhook))
                {
                    if (count >= settings.WebhookCount)
                    {
                        var hook = new DiscordWebhook();
                        hook.Uri = new Uri(settings.DiscordNotificationWebhook);

                        var message = new DiscordMessage();

                        message.Content = String.Format("{0}: Logged in {1}/{2}", serverName, count, teamMax * 2);

                        try
                        {
                            await hook.SendAsync(message);
                        }
                        catch { }
                    }
                }
            }
        }

        public async Task SendDiscordEndGameNotification(string serverName)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNotificationWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNotificationWebhook);

                    var message = new DiscordMessage();

                    message.Content = String.Format("{0}: Game ended", serverName);
                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordStartGameNotification(string serverName)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNotificationWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNotificationWebhook);

                    var message = new DiscordMessage();

                    message.Content = String.Format("{0}: Game started", serverName);

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordResignedNotification(string serverName){
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNotificationWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNotificationWebhook);

                    var message = new DiscordMessage();

                    message.Content = String.Format("{0}: Game resigned", serverName);

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordCanceledNotification(string serverName)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNotificationWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNotificationWebhook);

                    var message = new DiscordMessage();

                    message.Content = String.Format("{0}: Game canceled", serverName);

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }
            }
        }
    }
}
