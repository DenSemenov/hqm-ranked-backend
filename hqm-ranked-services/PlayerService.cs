using FirebaseAdmin.Messaging;
using Hangfire;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_helpers;
using hqm_ranked_models.DTO;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Serilog;
using SixLabors.ImageSharp;
using SpotifyAPI.Web.Http;
using System.CodeDom;
using System.Net;
using System.Runtime;

namespace hqm_ranked_backend.Services
{
    public class PlayerService : IPlayerService
    {
        private RankedDb _dbContext;
        private IImageGeneratorService _imageGeneratorService;
        private IStorageService _storageService;
        private INotificationService _notificationService;
        public PlayerService(RankedDb dbContext, IImageGeneratorService imageGeneratorService, IStorageService storageService, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _imageGeneratorService = imageGeneratorService;
            _storageService = storageService;
            _notificationService = notificationService;
        }
        public async Task<LoginResult?> Login(LoginRequest request)
        {
            var password = Encryption.GetMD5Hash(request.Password.Trim());

            var player = await _dbContext.Players.Include(x => x.Role).SingleOrDefaultAsync(x => x.Name == request.Login.Trim() && x.Password == password);
            if (player != null)
            {
                var token = Encryption.GetToken(player.Id, player.Role.Name == "admin");

                return new LoginResult
                {
                    Id = player.Id,
                    Success = true,
                    Token = token
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<LoginResult?> LoginWithDiscord(DiscordAuthRequest request)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = "https://discordapp.com/api/users/@me";
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + request.Token);
                    var res = await client.GetStringAsync(url);

                    var discordResult = JsonConvert.DeserializeObject<DiscordResult>(res);

                    var player = await _dbContext.Players.Include(x => x.Role).SingleOrDefaultAsync(x => x.DiscordId == discordResult.id);
                    if (player != null)
                    {
                        var token = Encryption.GetToken(player.Id, player.Role.Name == "admin");

                        return new LoginResult
                        {
                            Id = player.Id,
                            Success = true,
                            Token = token
                        };
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch {
                return null;
            }
        }

        public async Task<LoginResult?> Register(RegistrationRequest request)
        {
            try
            {
                var user = await _dbContext.Players.FirstOrDefaultAsync(x => x.Name == request.Login.Trim());

                if (user != null)
                {
                    return new LoginResult
                    {
                        Id = 0,
                        Success = false,
                        Token = String.Empty,
                        IsExists = true
                    };
                }

                var password = Encryption.GetMD5Hash(request.Password.Trim());
                var userRole = await _dbContext.Roles.SingleOrDefaultAsync(x => x.Name == "user");

                var approveRequired = _dbContext.Settings.FirstOrDefault().NewPlayerApproveRequired;

                var entity = await _dbContext.Players.AddAsync(new Player
                {
                    Id = _dbContext.Players.Max(x => x.Id) + 1,
                    Name = request.Login.Trim(),
                    Email = request.Email.Trim(),
                    Password = password,
                    Role = userRole,
                    IsApproved = approveRequired ? false : true,
                });
                await _dbContext.SaveChangesAsync();

                var token = Encryption.GetToken(entity.Entity.Id, userRole.Name == "admin");

                var path = String.Format("images/{0}.png", entity.Entity.Id);
                var file = _imageGeneratorService.GenerateImage();
                var strm = new MemoryStream();
                file.SaveAsPng(strm);

                await _storageService.UploadFileStream(path, strm);


                return new LoginResult
                {
                    Id = entity.Entity.Id,
                    Success = true,
                    Token = token,
                    IsExists = false
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Id = 0,
                    Success = false,
                    Token = ex.Message,
                };
            }
        }

        public async Task<CurrentUserVIewModel> GetCurrentUser(int userId, string ip, string userAgent, string acceptLang, string browser, string platform)
        {
            var result = new CurrentUserVIewModel();

            var user = await _dbContext.Players.Include(x => x.Bans).Include(x => x.Role).SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                result.Id = user.Id;
                result.Name = user.Name;
                result.Email = user.Email;
                result.Role = user.Role.Name;
                result.IsAcceptedRules = user.IsAcceptedRules;
                result.DiscordLogin = user.DiscordNickname;
                result.ShowLocation = user.ShowLocation;
                result.LimitType = user.LimitType;
                result.LimitTypeValue = user.LimitTypeValue;

                var approveRequired = _dbContext.Settings.FirstOrDefault().NewPlayerApproveRequired;

                result.IsApproved = approveRequired ? result.IsApproved : true;
                result.IsBanned = user.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow);
                if (result.IsBanned)
                {
                    var lastBan = user.Bans.OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                    result.BanLastDate = lastBan.CreatedOn.AddDays(lastBan.Days);
                }

                BackgroundJob.Enqueue(() => this.PutServerPlayerInfo(user.Id, ip, hqm_ranked_database.DbModels.LoginInstance.Web, userAgent, acceptLang, browser??String.Empty, platform ?? String.Empty));

                await _dbContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task ChangePassword(PasswordChangeRequest request, int userId)
        {
            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var encryptedPassword = Encryption.GetMD5Hash(request.Password);
                user.Password = encryptedPassword;
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<string> ChangeNickname(NicknameChangeRequest request, int userId)
        {
            var result = String.Empty;

            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                if (await _dbContext.Players.AnyAsync(x=>x.Name == request.Name.Trim()))
                {
                    result = String.Format("Nickname exists");
                }
                else
                {
                    var settings = await _dbContext.Settings.FirstOrDefaultAsync();
                    var countDays = settings.NicknameChangeDaysLimit;

                    if (_dbContext.NicknameChanges.Any(x => x.Player == user && x.CreatedOn.AddDays(countDays) > DateTime.UtcNow))
                    {
                        result = String.Format("You can change nickname each {0} days", countDays);
                    }
                    else
                    {
                        _dbContext.NicknameChanges.Add(new NicknameChanges
                        {
                            Player = user,
                            OldNickname = user.Name.Trim()
                        });

                        await _notificationService.SendDiscordNicknameChange(user, user.Name.Trim());

                        user.Name = request.Name.Trim();

                        result = "Nickname successfully changed";
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }

            return result;
        }

        public async Task ChangeLimitType(LimitTypeRequest request, int userId)
        {
            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                user.LimitTypeValue = request.LimitTypeValue;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AddPushToken(PushTokenRequest request, int userId)
        {
            var notification = await _dbContext.PlayerNotifications.Include(x => x.Player).SingleOrDefaultAsync(x => x.Player.Id == userId);
            if (notification != null)
            {
                notification.Token = request.Token;
            }
            else
            {
                var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
                if (player != null)
                {
                    _dbContext.PlayerNotifications.Add(new PlayerNotification
                    {
                        Player = player,
                        Token = request.Token
                    });
                }

            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemovePushToken(PushTokenRequest request, int userId)
        {
            var notification = await _dbContext.PlayerNotifications.Include(x => x.Player).SingleOrDefaultAsync(x => x.Player.Id == userId);
            if (notification != null)
            {
                _dbContext.PlayerNotifications.Remove(notification);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<PlayerNotificationsViewModel> GetPlayerNotifications(int userId)
        {
            var result = new PlayerNotificationsViewModel();

            var notification = await _dbContext.PlayerNotifications.Include(x => x.Player).SingleOrDefaultAsync(x => x.Player.Id == userId);
            if (notification != null)
            {
                if (notification != null)
                {
                    result.Token = notification.Token;
                    result.GameStarted = notification.GameStarted;
                    result.GameEnded = notification.GameEnded;
                    result.LogsCount = notification.LogsCount;
                }
            }

            return result;
        }

        public async Task SavePlayerNotifications(int userId, PlayerNotificationsViewModel request)
        {
            var notification = await _dbContext.PlayerNotifications.Include(x => x.Player).SingleOrDefaultAsync(x => x.Player.Id == userId);
            if (notification != null)
            {
                notification.GameStarted = request.GameStarted;
                notification.GameEnded = request.GameEnded;
                notification.LogsCount = request.LogsCount;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AcceptRules(int userId)
        {
            var player = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (player != null)
            {
                player.IsAcceptedRules = true;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<WebsiteSettingsViewModel> GetWebsiteSettings()
        {
            var result = new WebsiteSettingsViewModel();

            var settings = await _dbContext.Settings.FirstOrDefaultAsync();
            if (settings != null)
            {
                result.DiscordAppClientId = settings.DiscordAppClientId;
                result.DiscordJoinLink = settings.DiscordJoinLink;
            }

            return result;
        }

        public async Task SetDiscordByToken(int userId, string token)
        {
            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var url = "https://discordapp.com/api/users/@me";
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                        var res = await client.GetStringAsync(url);

                        var discordResult = JsonConvert.DeserializeObject<DiscordResult>(res);

                        user.DiscordId = discordResult.id;
                        user.DiscordNickname = discordResult.username;

                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch { }
            }
        }

        public async Task RemoveDiscord(int userId)
        {
            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                user.DiscordId = String.Empty;
                user.DiscordNickname = String.Empty;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<PlayerWarningViewModel>> GetPlayerWarnings(int userId)
        {
            var result = new List<PlayerWarningViewModel>();

            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                if (String.IsNullOrEmpty(user.DiscordId))
                {
                    result.Add(new PlayerWarningViewModel
                    {
                        Type = WarningType.DiscordNotConnected,
                        Message = "Connect your Discord account to avoid losing access to it if you forget your password"
                    });
                }
            }

            return result;
        }

        public async Task<PlayerLoginInfo> GetIpInfo(string ip)
        {
            var url = "http://ip-api.com/json/" + ip;
            var info = new WebClient().DownloadString(url);
            var ipInfo = JsonConvert.DeserializeObject<PlayerLoginInfo>(info);

            return ipInfo;
        }

        public async Task PutServerPlayerInfo(int playerId, string ip, hqm_ranked_database.DbModels.LoginInstance loginInstance, string userAgent, string acceptLang, string browser, string platform)
        {
            var user = await _dbContext.Players.Include(x=>x.PlayerLogins).FirstOrDefaultAsync(x => x.Id == playerId);
            if (user != null)
            {
                try
                {

                    var url = "http://ip-api.com/json/" + ip;
                    var info = new WebClient().DownloadString(url);
                    var ipInfo = JsonConvert.DeserializeObject<PlayerLoginInfo>(info);

                    user.PlayerLogins.Add(new hqm_ranked_database.DbModels.PlayerLogin
                    {
                        City = ipInfo.city,
                        CountryCode = ipInfo.countryCode,
                        Ip = ip,
                        LoginInstance = loginInstance,
                        UserAgent = userAgent,
                        AcceptLang = acceptLang,
                        Browser = browser,
                        Platform = platform,
                        Lat = ipInfo.lat,
                        Lon = ipInfo.lon
                    });
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(LogHelper.GetErrorLog(ex.Message, ex.StackTrace));
                }
            }
        }

        public async Task SetShowLocation(SetShowLocationRequest request, int userId)
        {
            var user = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                user.ShowLocation = request.ShowLocation;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<PlayerMapViewModel>> GetMap()
        {
            var result = new List<PlayerMapViewModel>();
            var players = await _dbContext.Players.Include(x => x.PlayerLogins).Where(x => x.PlayerLogins.Count > 0).Select(x => new
            {
                PlayerId = x.ShowLocation ? x.Id : 0,
                PlayerName = x.ShowLocation ? x.Name : String.Empty,
                IsHidden = !x.ShowLocation,
                Locations = x.PlayerLogins.OrderByDescending(x => x.CreatedOn).Where(x => x.Lat != 0).Take(100).Select(x=>new
                {
                    x.Lon,
                    x.Lat
                }).ToList()
            }).ToListAsync();

            var rnd = new Random();
            foreach (var player in players)
            {
                var location = player.Locations.GroupBy(x => new { x.Lon, x.Lat }).OrderByDescending(x=>x.Count()).FirstOrDefault();

                result.Add(new PlayerMapViewModel
                {
                    PlayerId = player.PlayerId == 0 ? rnd.Next(): player.PlayerId,
                    PlayerName = player.PlayerName,
                    IsHidden = player.IsHidden,
                    Lon = location.Key.Lon,
                    Lat = location.Key.Lat,
                });
            }

            return result;
        }

        public async Task FillLocations()
        {
            var logins = await _dbContext.PlayerLogins.Where(x => x.Lat == 0).ToListAsync();
            var ips = logins.Select(x => x.Ip).Distinct().ToList();

            var chunks = ips.Chunk(100);

            var results = new List<PlayerLoginInfo>();

            foreach (var chunk in chunks)
            {
                var json = JsonConvert.SerializeObject(chunk);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://ip-api.com/batch?fields=query,lat,lon");
                var content = new StringContent(json, null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var r = JsonConvert.DeserializeObject<PlayerLoginInfo[]>(result);
                results.AddRange(r);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            foreach (var item in results)
            {
                var foundLogins = logins.Where(x => x.Ip == item.query);
                foreach (var foundLogin in foundLogins)
                {
                    foundLogin.Lat = item.lat;
                    foundLogin.Lon = item.lon;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task CalcPlayersStats()
        {
            var players = await _dbContext.Players.Include(x => x.PlayerCalcStats).ToListAsync();

            var tempStats = new List<PlayerCalcModel>();

            foreach (var player in players)
            {
                var last100Games = await _dbContext.GamePlayers.Include(x => x.Game).Where(x => x.PlayerId == player.Id).OrderByDescending(x => x.CreatedOn).Take(100).Select(x=>new
                {
                    MvpId = x.Game.MvpId,
                    Goals = x.Goals,
                    Assists = x.Assists,
                    Team = x.Team,
                    RedScore = x.Game.RedScore,
                    BlueScore = x.Game.BlueScore,
                    Shots = x.Shots,
                    Saves = x.Saves,
                    Conceded = x.Conceded
                }).ToListAsync();

                if (last100Games.Count >= 100)
                {
                    tempStats.Add(new PlayerCalcModel
                    {
                        PlayerId = player.Id,
                        Mvp = last100Games.Count(x => x.MvpId == player.Id) / (double)100,
                        Gpg = last100Games.Sum(x => x.Goals) / (double)100,
                        Apg = last100Games.Sum(x => x.Assists) / (double)100,
                        Winrate = last100Games.Count(x => (x.Team == 0 && x.RedScore > x.BlueScore) || (x.Team == 1 && x.RedScore < x.BlueScore)) / (double)100,
                        Shots = last100Games.Sum(x => x.Goals) / (double)last100Games.Sum(x => x.Shots < x.Goals ? x.Goals : x.Shots),
                        Saves = last100Games.Sum(x => x.Saves) / (double)last100Games.Sum(x => x.Conceded + x.Saves)
                    });
                }
            }

            var minMvp = tempStats.Min(x => x.Mvp);
            var maxMvp = tempStats.Max(x => x.Mvp);

            var minGpg = tempStats.Min(x => x.Gpg);
            var maxGpg = tempStats.Max(x => x.Gpg);

            var minApg = tempStats.Min(x => x.Apg);
            var maxApg = tempStats.Max(x => x.Apg);

            var minWinrate = tempStats.Min(x => x.Winrate);
            var maxWinrate = tempStats.Max(x => x.Winrate);

            var minShots = tempStats.Min(x => x.Shots);
            var maxShots = tempStats.Max(x => x.Shots);

            var minSaves = tempStats.Min(x => x.Saves);
            var maxSaves = tempStats.Max(x => x.Saves);

            foreach (var player in players)
            {
                var tempStat = tempStats.FirstOrDefault(x => x.PlayerId == player.Id);
                if (tempStat != null)
                {
                    var mvpValue = PercentageInRangeCalc.CalculatePercentage(tempStat.Mvp, minMvp, maxMvp);
                    var gpgValue = PercentageInRangeCalc.CalculatePercentage(tempStat.Gpg, minGpg, maxGpg);
                    var apgValue = PercentageInRangeCalc.CalculatePercentage(tempStat.Apg, minApg, maxApg);
                    var winrateValue = PercentageInRangeCalc.CalculatePercentage(tempStat.Winrate, minWinrate, maxWinrate);
                    var shotsValue = PercentageInRangeCalc.CalculatePercentage(tempStat.Shots, minShots, maxShots);
                    var savesValue = PercentageInRangeCalc.CalculatePercentage(tempStat.Saves, minSaves, maxSaves);

                    if (player.PlayerCalcStats != null)
                    {
                        player.PlayerCalcStats.Mvp = mvpValue;
                        player.PlayerCalcStats.Goals = gpgValue;
                        player.PlayerCalcStats.Assists = apgValue;
                        player.PlayerCalcStats.Winrate = winrateValue;
                        player.PlayerCalcStats.Shots = shotsValue;
                        player.PlayerCalcStats.Saves = savesValue;
                    }
                    else
                    {
                        player.PlayerCalcStats = new hqm_ranked_database.DbModels.PlayerCalcStats
                        {
                            Mvp = mvpValue,
                            Goals = gpgValue,
                            Assists = apgValue,
                            Winrate = winrateValue,
                            Shots = shotsValue,
                            Saves = savesValue
                        };
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
