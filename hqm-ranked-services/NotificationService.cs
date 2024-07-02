using CSharpDiscordWebhook.NET.Discord;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_services.Helpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using TimeAgo;

namespace hqm_ranked_backend.Services
{
    public class NotificationService : INotificationService
    {
        private RankedDb _dbContext;
        public NotificationService(RankedDb dbContext)
        {
            _dbContext = dbContext;

            var settings = _dbContext.Settings.FirstOrDefault();
            if (FirebaseApp.DefaultInstance == null)
            {
                try
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromJson(settings.PushJson),
                        ProjectId = "hqmpush"
                    });
                }
                catch { }
            }
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

        public async Task SendDiscordTeamsGame(GameInvites gameInvite, string team1, string team2)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();

                    var timeAgo = TimeHelper.GetRemainingTime(gameInvite.Date);

                    var desc = String.Format("{0} vs {1} {2} ({3})", team1, team2, timeAgo, gameInvite.GamesCount);

                    message.Username = "TEAMS";
                    message.Embeds.Add(new DiscordEmbed
                    {
                        Title = desc,
                    });

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public async Task SendDiscordTeamInvite(GameInvites gameInvite)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();

                    var timeAgo = TimeHelper.GetRemainingTime( gameInvite.Date);

                    var desc = String.Format("New game invite available {0} ({1} games)", timeAgo, gameInvite.GamesCount);

                    message.Username = "TEAMS";
                    message.Embeds.Add(new DiscordEmbed
                    {
                        Title = desc,
                    });

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public async Task SendDiscordNicknameChange(Player player, string oldNickname)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();

                    var desc = String.Format("New nickname: {0}", player.Name);

                    var storageUrl = String.Format("https://{0}/{1}/{2}/", settings.S3Domain, settings.S3Bucket, settings.Id);
                    var imageUrl = String.Format("{0}images/{1}.png", storageUrl, player.Id);

                    message.Username = oldNickname;
                    message.AvatarUrl = new Uri(imageUrl);

                    message.Embeds.Add(new DiscordEmbed
                    {
                        Title = desc,
                    });

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }


        public async Task SendDiscordNewsAward(Award award, string playerName)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();

                    var desc = String.Empty;

                    switch (award.AwardType)
                    {
                        case AwardType.FirstPlace:
                            desc = String.Format("First place ({0})", award.Season.Name);
                            break;
                        case AwardType.SecondPlace:
                            desc = String.Format("Second place ({0})", award.Season.Name);
                            break;
                        case AwardType.ThirdPlace:
                            desc = String.Format("Third place ({0})", award.Season.Name);
                            break;
                        case AwardType.BestGoaleador:
                            desc = String.Format("Best goaleador ({0})", award.Season.Name);
                            break;
                        case AwardType.BestAssistant:
                            desc = String.Format("Best assistant ({0})", award.Season.Name);
                            break;
                        case AwardType.GamesPlayed:
                            desc = String.Format("{0} games played", award.Count);
                            break;
                        case AwardType.Goals:
                            desc = String.Format("{0} goals", award.Count);
                            break;
                        case AwardType.Assists:
                            desc = String.Format("{0} assists", award.Count);
                            break;
                    }

                    var storageUrl = String.Format("https://{0}/{1}/{2}/", settings.S3Domain, settings.S3Bucket, settings.Id);
                    var imageUrl = String.Format("{0}images/{1}.png", storageUrl, award.PlayerId);

                    var title = String.Format("{0} {1}", award.Season != null ? "🏆" : "🏅", playerName);

                    message.Username = title;
                    message.AvatarUrl = new Uri(imageUrl);

                    message.Embeds.Add(new DiscordEmbed
                    {
                        Title = desc,
                    });

                    try
                    {
                        await hook.SendAsync(message);
                        Thread.Sleep(2000);
                    }
                    catch (Exception ex)
                    {

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

        public async Task SendPush(string title, string body, List<string> tokens)
        {
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = tokens,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
