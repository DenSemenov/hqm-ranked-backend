using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class AdminService: IAdminService
    {
        private RankedDb _dbContext;
        public AdminService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AdminServerViewModel>> GetServers()
        {
            var result = await _dbContext.Servers.OrderBy(x => x.CreatedOn).Select(x => new AdminServerViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Token = x.Token,
                InstanceType = x.InstanceType
            }).ToListAsync();

            return result;
        }

        public async Task AddServer(AddServerRequest request)
        {
            await _dbContext.Servers.AddAsync(new Server
            {
                Name = request.Name,
                Token = Guid.NewGuid().ToString(),
                InstanceType = request.InstanceType
            });
            await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveServer(RemoveServerRequest request)
        {
            var server = await _dbContext.Servers.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (server != null)
            {
                _dbContext.Servers.Remove(server);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<AdminPlayerViewModel>> GetPlayers()
        {
            return await _dbContext.Players.Include(x => x.Bans).Select(x => new AdminPlayerViewModel
            {
                Id = x.Id,
                Name = x.Name,
                IsBanned = x.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow)
            }).ToListAsync();
        }

        public async Task BanPlayer(BanUnbanRequest request)
        {
            if (request.IsBanned)
            {
                var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == request.Id);
                _dbContext.Bans.Add(new Bans
                {
                    BannedPlayer = player,
                    Days = request.Days,
                });
            }
            else
            {
                var ban = await _dbContext.Bans.Include(x=>x.BannedPlayer).OrderByDescending(x=>x.CreatedOn).FirstOrDefaultAsync(x=>x.BannedPlayer.Id == request.Id);
                _dbContext.Remove(ban);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<AdminViewModel>> GetAdmins()
        {
            var result = await _dbContext.Players.Include(x=>x.Role).Where(x=>x.Role.Name == "admin").Select(x=>new AdminViewModel
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();

            return result;
        }

        public async Task AddRemoveAdmin(AddRemoveAdminRequest request)
        {
            var player = await _dbContext.Players.Include(x => x.Role).FirstOrDefaultAsync(x=>x.Id == request.Id);
            if (player != null)
            {
                if (player.Role.Name != "admin")
                {
                    player.Role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == "admin");
                }
                else
                {
                    player.Role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == "user");
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Setting> GetSettings()
        {
            return await _dbContext.Settings.FirstOrDefaultAsync();
        }

        public async Task SaveSettings(Setting request)
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync(); 
            if (settings != null)
            {
                settings.NewPlayerApproveRequired = request.NewPlayerApproveRequired;
                settings.NicknameChangeDaysLimit = request.NicknameChangeDaysLimit;
                settings.Rules = request.Rules;
                settings.ReplayStoreDays = request.ReplayStoreDays;
                settings.NextGameCheckGames = request.NextGameCheckGames;
                settings.DiscordNotificationWebhook = request.DiscordNotificationWebhook;
                settings.DiscordAppClientId = request.DiscordAppClientId;
                settings.DiscordNewsWebhook = request.DiscordNewsWebhook;
                settings.WebhookCount = request.WebhookCount;
                settings.ShadowBanReportsCount = request.ShadowBanReportsCount;
                settings.StartingElo = request.StartingElo;
                settings.S3Domain = request.S3Domain;
                settings.S3Bucket = request.S3Bucket;
                settings.S3User = request.S3User;
                settings.S3Key = request.S3Key;
                settings.TeamsMaxPlayer = request.TeamsMaxPlayer;
                settings.SpotifyPlaylist = request.SpotifyPlaylist;
                settings.SpotifyClientId = request.SpotifyClientId;
                settings.SpotifySecret= request.SpotifySecret;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<AdminPlayerViewModel>> GetUnApprovedUsers()
        {
            return await _dbContext.Players.Where(x => x.IsApproved == false).Select(x=>new AdminPlayerViewModel
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();
        }

        public async Task ApproveUser(IApproveRequest request)
        {
            var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (player != null)
            {
                player.IsApproved = true;

                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task AddAdminStory(AdminStoryRequest request)
        {
            _dbContext.AdminStories.Add(new AdminStory
            {
                Text = request.Text,
                Expiration = request.Expiration,
                Link = request.Link,
            });

            await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveAdminStory(RemoveAdminStoryRequest request)
        {
            var story = await _dbContext.AdminStories.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (story != null)
            {
                _dbContext.AdminStories.Remove(story);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<AdminStoryViewModel>> GetAdminStories()
        {
            var dateDayBefore = DateTime.UtcNow.AddDays(-1);

            var result = await _dbContext.AdminStories.Where(x=>(x.Expiration && x.CreatedOn > dateDayBefore) || !x.Expiration).OrderByDescending(x => x.CreatedOn).Select(x => new AdminStoryViewModel
            {
                Id = x.Id,
                Date = x.CreatedOn,
                Text = x.Text,
                Expiration = x.Expiration,
                Link = x.Link,
            }).ToListAsync();
            return result;
        }
    }
}
