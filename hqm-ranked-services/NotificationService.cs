using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using CSharpDiscordWebhook.NET.Discord;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_models.DTO;
using hqm_ranked_services.Helpers;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static SpotifyAPI.Web.PlaylistRemoveItemsRequest;

namespace hqm_ranked_backend.Services
{
    public class NotificationService : INotificationService
    {
        private RankedDb _dbContext;
        private static TelegramBotClient? _telegramBot;
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

            if (!String.IsNullOrEmpty(settings.TelegramBotToken))
            {
                _telegramBot = new TelegramBotClient(settings.TelegramBotToken);
            }
        }

        //public async Task GetNews()
        //{

        //    var msgs = new List<MessageItem>();
        //    try
        //    {
        //        var token = "Bot ";
        //        var client = new HttpClient();
        //        var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/v10/channels/1257624019067207780/messages?limit=100");
        //        request.Headers.Add("Accept", "application/json");
        //        request.Headers.Add("Authorization", token);
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode(); 
        //        var res = await response.Content.ReadAsStringAsync();

        //        var myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(res);

        //        foreach(var item in myDeserializedClass)
        //        {
        //            var type = MsgType.Achive;
        //            var title = String.Empty;
        //            var desc = String.Empty;
        //            var images = new List<string>();

        //            var firstEmbedTitle = item.embeds.Any() ? item.embeds.FirstOrDefault().title?? String.Empty: String.Empty;

        //            if (firstEmbedTitle.StartsWith("New nickname"))
        //            {
        //                type = MsgType.NewNickname;
        //                title = item.author.username;
        //                desc = firstEmbedTitle;
        //            }
        //            else if (firstEmbedTitle.EndsWith(" games played") || firstEmbedTitle.EndsWith(" goals") || firstEmbedTitle.EndsWith(" assists") || firstEmbedTitle.StartsWith("Best assistant ") || firstEmbedTitle.StartsWith("Best goaleador ") || firstEmbedTitle.StartsWith("Third place ") || firstEmbedTitle.StartsWith("Second place ") || firstEmbedTitle.StartsWith("First place "))
        //            {
        //                type = MsgType.Achive;
        //                title = item.author.username;
        //                desc = firstEmbedTitle;
        //            }
        //            else if (item.content.StartsWith("Registration started"))
        //            {
        //                title = item.content;
        //                type = MsgType.WtRegistrationStarted;
        //            }
        //            else if (item.content.StartsWith("Tourney started"))
        //            {
        //                title = item.content;
        //                type = MsgType.WtStarted;

        //                foreach(var em in item.embeds)
        //                {
        //                    desc += "*" + em.author.name+ "*" + Environment.NewLine + Regex.Replace(em.description, @"<@![^>]+>", "") + Environment.NewLine+ Environment.NewLine;
        //                }
        //            }
        //            else if (item.content.StartsWith("Next games"))
        //            {
        //                title = item.content;
        //                type = MsgType.WtGames;
        //                images.Add(item.embeds.FirstOrDefault().image.url);

        //            }
        //            else if (firstEmbedTitle.StartsWith("Congratulations to the winner!"))
        //            {
        //                type = MsgType.WtWinner;
        //                title = firstEmbedTitle;
        //                desc = Regex.Replace(item.embeds.FirstOrDefault().description, @"<@![^>]+>", ""); 
        //            }

        //            if (!String.IsNullOrEmpty(title))
        //            {
        //                msgs.Add(new MessageItem
        //                {
        //                    Type = type,
        //                    Date = item.timestamp,
        //                    Title = title,
        //                    Description = desc,
        //                    Images = images
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    var settings = await _dbContext.Settings.FirstOrDefaultAsync();

        //    foreach (var item in msgs.OrderBy(x=>x.Date).TakeLast(1))
        //    {
        //        if (_telegramBot != null)
        //        {
        //            try
        //            {
        //                var text = EscapeMarkdownV2("*" + item.Title + "*" + Environment.NewLine + item.Description);

        //                await _telegramBot.SendTextMessageAsync(
        //                    chatId: settings.TelegramGroupId,
        //                    text: text,
        //                    replyToMessageId: settings.NewsThreadId,
        //                    parseMode: ParseMode.MarkdownV2
        //                );

        //                foreach (var image in item.Images)
        //                {
        //                    var file = InputFile.FromUri(image);
        //                    await _telegramBot.SendPhotoAsync(
        //                        chatId: settings.TelegramGroupId,
        //                        replyToMessageId: settings.NewsThreadId,
        //                         photo: file
        //                         );
        //                }
        //            }
        //            catch (Exception ex )
        //            {

        //            }
        //        }

        //        Thread.Sleep(3000);
        //    }
        //}

        public string EscapeMarkdownV2(string text)
        {
            string[] reservedCharacters = { "_",  "[", "]", "(", ")", "~", "`", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
            foreach (var ch in reservedCharacters)
            {
                text = text.Replace(ch, "\\" + ch);
            }
            return text;
        }

        //public class MessageItem
        //{
        //    public MsgType Type { get; set; }
        //    public string Title { get; set; }
        //    public string Description { get; set; }
        //    public DateTime Date { get; set; }
        //    public List<string> Images { get; set; }
        //}

        //public enum MsgType
        //{
        //    Achive,
        //    WtWinner,
        //    WtGames,
        //    WtStarted,
        //    WtRegistrationStarted,
        //    NewNickname,

        //}

        //// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        //public class Author
        //{
        //    public string username { get; set; }
        //    public string name { get; set; }
        //}

        //public class Img
        //{
        //    public string url { get; set; }
        //}
        //public class Embed
        //{
        //    public string type { get; set; }
        //    public string title { get; set; }
        //    public int content_scan_version { get; set; }
        //    public string description { get; set; }
        //    public Img image { get; set; }
        //    public Author author { get; set; }
        //    public string url { get; set; }
        //}

        //public class Root
        //{
        //    public string content { get; set; }
        //    public List<object> attachments { get; set; }
        //    public List<Embed> embeds { get; set; }
        //    public DateTime timestamp { get; set; }
           
        //    public Author author { get; set; }
        //}

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

                if (_telegramBot != null)
                {
                    if (count >= settings.WebhookCount)
                    {
                        try
                        {
                            await _telegramBot.SendTextMessageAsync(
                                chatId: settings.TelegramGroupId,
                                text: String.Format("{0}: Logged in {1}/{2}", serverName, count, teamMax * 2),
                                replyToMessageId: settings.NotificationThreadId
                            );
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

                if (_telegramBot != null)
                {
                    try
                    {
                        var timeAgo = TimeHelper.GetRemainingTime(gameInvite.Date);
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: String.Format("{0} vs {1} {2} ({3})", team1, team2, timeAgo, gameInvite.GamesCount),
                            replyToMessageId: settings.NewsThreadId
                        );
                    }
                    catch { }
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

                if (_telegramBot != null)
                {
                    try
                    {
                        var timeAgo = TimeHelper.GetRemainingTime(gameInvite.Date);
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: String.Format("New game invite available {0} ({1} games)", timeAgo, gameInvite.GamesCount),
                            replyToMessageId: settings.NewsThreadId
                        );
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordNicknameChange(Player player, string newNickname)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();

                    var desc = String.Format("New nickname: {0}", newNickname);

                    var storageUrl = String.Format("https://{0}/{1}/{2}/", settings.S3Domain, settings.S3Bucket, settings.Id);
                    var imageUrl = String.Format("{0}images/{1}.png", storageUrl, player.Id);

                    message.Username = player.Name;
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

                if (_telegramBot !=null)
                {
                    try
                    {
                        var desc = String.Format("New nickname: {0}", newNickname);
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: EscapeMarkdownV2("*" + player.Name + "*" + Environment.NewLine + desc),
                            replyToMessageId: settings.NewsThreadId,
                            parseMode: ParseMode.MarkdownV2
                        );
                    }
                    catch { }
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

                if (_telegramBot != null)
                {
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

                    var title = String.Format("{0} {1}", award.Season != null ? "🏆" : "🏅", playerName);

                    try
                    {
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: EscapeMarkdownV2("*" + title + "*" + Environment.NewLine + desc),
                            replyToMessageId: settings.NewsThreadId,
                            parseMode: ParseMode.MarkdownV2
                        );
                    }
                    catch { }
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

                if (_telegramBot != null)
                {
                    try
                    {
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: String.Format("{0}: Game ended", serverName),
                            replyToMessageId: settings.NotificationThreadId
                        );
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordStartGameNotification(string serverName, List<string> ids)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNotificationWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNotificationWebhook);

                    var message = new DiscordMessage();

                    message.Content = String.Format("{0}: Game started\n{1}", serverName, String.Join(", ", ids.Select(x => "<@!" + x + ">")));

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }

                if (_telegramBot != null)
                {
                    try
                    {
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: String.Format("{0}: Game started", serverName),
                            replyToMessageId: settings.NotificationThreadId
                        );
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
        public async Task SendDiscordRegistrationStarted(string name, Guid id)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();
                    message.Username = name ;
                    var url = settings.WebUrl + "/weekly-tourney?id=" + id;
                    message.Content = "Registration started" + Environment.NewLine + url;


                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }

                if (_telegramBot != null)
                {
                    try
                    {
                        var url = settings.WebUrl + "/weekly-tourney?id=" + id;
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: EscapeMarkdownV2("*Registration started*" + Environment.NewLine + url),
                            replyToMessageId: settings.NewsThreadId,
                            parseMode: ParseMode.MarkdownV2
                        );
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordTourneyStarted(string name, Guid id, List<TourneyStartedDTO> teams)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();
                    message.Username = name;
                    var url = settings.WebUrl + "/weekly-tourney?id=" + id;
                    message.Content = "Tourney started" + Environment.NewLine + url;

                    foreach (var team in teams)
                    {
                        message.Embeds.Add(new DiscordEmbed
                        {
                            Author = new EmbedAuthor
                            {
                                Name = team.Name,
                                IconUrl = new Uri(team.AvatarUrl)
                            },
                            Description = String.Join(Environment.NewLine, team.Players.Select(x => String.IsNullOrEmpty(x.DiscordId) ? x.Name : String.Format("{0} <@!{1}>", x.Name, x.DiscordId)))
                        });
                    }

                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }

                if (_telegramBot != null)
                {
                    try
                    {
                        var url = settings.WebUrl + "/weekly-tourney?id=" + id;
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: EscapeMarkdownV2("*Tourney started*" + Environment.NewLine + url),
                            replyToMessageId: settings.NewsThreadId
                        );
                    }
                    catch { }
                }
            }
        }

        public async Task SendDiscordTourneyGames(string name, Guid id, string imageUrl)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.DiscordNewsWebhook))
                {
                    var hook = new DiscordWebhook();
                    hook.Uri = new Uri(settings.DiscordNewsWebhook);

                    var message = new DiscordMessage();
                    message.Username = name;
                    var url = settings.WebUrl + "/weekly-tourney?id=" + id;
                    message.Content = "Next games" + Environment.NewLine + url;
                    message.Embeds.Add(new DiscordEmbed
                    {
                         Image = new EmbedMedia
                         {
                              Url = new Uri(imageUrl)
                         }
                    });
                    try
                    {
                        await hook.SendAsync(message);
                    }
                    catch { }
                }

                if (_telegramBot != null)
                {
                    try
                    {
                        var url = settings.WebUrl + "/weekly-tourney?id=" + id;
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: EscapeMarkdownV2("*Next games*" + Environment.NewLine + url),
                            replyToMessageId: settings.NewsThreadId
                        );

                        var file = InputFile.FromUri(imageUrl);
                        await _telegramBot.SendPhotoAsync(
                            chatId: settings.TelegramGroupId,
                            replyToMessageId: settings.NewsThreadId,
                             photo: file
                       );
                    }
                    catch (Exception ex)
                    {

                    }
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

                if (_telegramBot != null)
                {
                    try
                    {
                        await _telegramBot.SendTextMessageAsync(
                            chatId: settings.TelegramGroupId,
                            text: String.Format("{0}: Game canceled", serverName),
                            replyToMessageId: settings.NewsThreadId
                        );
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
