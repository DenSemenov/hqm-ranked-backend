using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static MassTransit.ValidationResultExtensions;

namespace hqm_ranked_backend.Services
{
    public class SeasonService: ISeasonService
    {
        private RankedDb _dbContext;
        public SeasonService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SeasonViewModel>> GetSeasons()
        {
            var result = await _dbContext.Seasons.Select(x => new SeasonViewModel
            {
                Id = x.Id,
                Name = x.Name,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd
            }).OrderByDescending(x=>x.DateStart).ToListAsync();

            return result;
        }

        public async Task<Season> GetCurrentSeason()
        {
            var result = await _dbContext.Seasons.FirstOrDefaultAsync(x => x.DateStart <= DateTime.UtcNow && x.DateEnd > DateTime.UtcNow);

            return result;
        }

        public async Task<List<SeasonStatsViewModel>> GetSeasonStats(CurrentSeasonStatsRequest request)
        {
            var result  = new List<SeasonStatsViewModel>();

            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");
            var startingElo = _dbContext.Settings.FirstOrDefault().StartingElo;

            var season = await _dbContext.Seasons.SingleOrDefaultAsync(x => x.Id == request.SeasonId);
            var games = _dbContext.GamePlayers
                .Include(x => x.Game)
                .Include(x => x.Player)
                .Where(x => x.Game.Season == season && (x.Game.State == ended || x.Game.State == resigned))
                .Select(x => new 
                {
                    PlayerId = x.Player.Id,
                    Nickname = x.Player.Name,
                    Goals = x.Goals,
                    Assists = x.Assists,
                    Win = x.Team == 0 ? x.Game.RedScore > x.Game.BlueScore: x.Game.RedScore < x.Game.BlueScore,
                    Lose = x.Team == 0 ? x.Game.RedScore < x.Game.BlueScore : x.Game.RedScore > x.Game.BlueScore,
                    Mvp = x.Game.Mvp == x.Player,
                    Score = x.Score 
                })
                .GroupBy(x => x.PlayerId)
                .ToList();

            var elos = await _dbContext.Elos.Include(x=>x.Player).Where(x => x.Season == season).ToListAsync();

            foreach(var game in games)
            {
                var elo = startingElo;

                var playerElo = elos.SingleOrDefault(x => x.Player.Id == game.Key && x.Season == season);
                if (playerElo != null)
                {
                    elo = playerElo.Value;
                }

                result.Add(new SeasonStatsViewModel
                {
                    PlayerId = game.Key,
                    Nickname = game.FirstOrDefault().Nickname,
                    Goals = game.Sum(x => x.Goals),
                    Assists = game.Sum(x => x.Assists),
                    Win = game.Count(x => x.Win),
                    Lose = game.Count(x => x.Lose),
                    Mvp = game.Count(x => x.Mvp),
                    Rating = game.Sum(x => x.Score) + elo
                });
            }

            result = result.OrderByDescending(x => x.Rating).ToList();

            var i = 1;
            foreach(var player in result)
            {
                player.Place = i;
                i++;
            }

            return result;
        }

        public async Task<List<SeasonGameViewModel>> GetSeasonGames(CurrentSeasonStatsRequest request)
        {
            var season = await _dbContext.Seasons.SingleOrDefaultAsync(x => x.Id == request.SeasonId);
            var games = await _dbContext.Games
                .Include(x => x.State)
                .Include(x => x.GamePlayers)
                .ThenInclude(x => x.Player)
                .Include(x => x.ReplayDatas)
                .ThenInclude(x => x.ReplayFragments)
                .Where(x => x.Season == season)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new SeasonGameViewModel
                {
                    GameId = x.Id,
                    Date = x.LastModifiedOn ?? x.CreatedOn,
                    RedScore = x.RedScore,
                    BlueScore = x.BlueScore,
                    Status = x.State.Name,
                    ReplayId = x.ReplayDatas.Any() ? x.ReplayDatas.FirstOrDefault().Id : null,
                    HasReplayFragments = x.ReplayDatas.Any() ? x.ReplayDatas.FirstOrDefault().ReplayFragments.Count != 0 : false,
                    Players = x.GamePlayers.Select(x => new GamePlayerItem
                    {
                        Id = x.PlayerId,
                        Name = x.Player.Name,
                        Team = x.Team
                    }).ToList()
                })
                .Skip(request.Offset)
                .Take(30)
                .ToListAsync();

            return games;
        }

        public async Task<PlayerViewModel> GetPlayerData(PlayerRequest request)
        {
            var result = new PlayerViewModel();

            result.Id = request.Id;

            var player = await _dbContext.Players.Include(x => x.GamePlayers).ThenInclude(x => x.Game).ThenInclude(x => x.GamePlayers).ThenInclude(x => x.Player).Include(x => x.NicknameChanges).Include(x=>x.Cost).Select(x =>
            new {
                Id = x.Id,
                Name = x.Name,
                Count = x.GamePlayers.Count,
                Goals = x.GamePlayers.Sum(x => x.Goals),
                Assists = x.GamePlayers.Sum(x => x.Assists),
                LastGames = x.GamePlayers.OrderByDescending(x => x.Game.CreatedOn).Take(3),
                NicknameChanges = x.NicknameChanges,
                Cost = x.Cost !=null? x.Cost.Cost: 0,
            }).SingleOrDefaultAsync(x => x.Id == request.Id);

            result.Name = player.Name;
            result.Games = player.Count;
            result.Goals = player.Goals;
            result.Assists = player.Assists;
            result.Points = result.Goals + result.Assists;
            result.Cost = player.Cost;

            var currentSeasonStats = await GetSeasons();
            var lastSeason = currentSeasonStats.FirstOrDefault();
            var seasonStats = await GetSeasonStats(new CurrentSeasonStatsRequest { SeasonId = lastSeason.Id });

            var currentPlayerStats = seasonStats.SingleOrDefault(x => x.PlayerId == request.Id);
            if (currentPlayerStats != null)
            {
                result.CurrentSeasonData = new PlayerLastSeasonViewModel
                {
                    Games = currentPlayerStats.Win + currentPlayerStats.Lose,
                    Goals = currentPlayerStats.Goals,
                    Assists = currentPlayerStats.Assists,
                    Points = currentPlayerStats.Goals + currentPlayerStats.Assists,
                    Position = seasonStats.IndexOf(currentPlayerStats) + 1,
                    Elo = currentPlayerStats.Rating
                };
            }

            var lastSeasons = currentSeasonStats.Skip(1).Take(3);
            foreach (var season in lastSeasons)
            {
                var seasonStatsTemp = await GetSeasonStats(new CurrentSeasonStatsRequest { SeasonId = season.Id });
                var lastSeasonStats = seasonStatsTemp.SingleOrDefault(x => x.PlayerId == request.Id);
                if (lastSeasonStats != null)
                {
                    result.LastSeasons.Add(new PlayerSeasonsViewModel
                    {
                        Name = season.Name,
                        Place = seasonStatsTemp.IndexOf(lastSeasonStats) + 1
                    });
                }
            }

            result.LastGames = player.LastGames.Select(x => new PlayerLastGamesViewModel
            {
                Date = x.CreatedOn,
                GameId = x.Game.Id,
                Goals = x.Goals,
                Assists = x.Assists,
                Score = x.Score,
                RedScore = x.Game.RedScore,
                BlueScore = x.Game.BlueScore,
                Players = x.Game.GamePlayers.Select(x => new GameDataPlayerViewModel
                {
                    Id = x.PlayerId,
                    Name = x.Player.Name,
                    Goals = x.Goals,
                    Assists = x.Assists,
                    Score = x.Score,
                    Team = x.Team,
                }).ToList()
            }).ToList();

            result.CalcData = new PlayerCalcDataViewModel();

            result.OldNicknames = player.NicknameChanges.OrderByDescending(x=>x.CreatedOn).Select(x=>x.OldNickname).ToList();

            return result;
        }

        public async Task<GameDataViewModel> GetGameData(GameRequest request)
        {
            var storageUrl = await GetStorage();

            var result = await _dbContext.Games
                .Include(x => x.State)
                .Include(x => x.GamePlayers)
                .ThenInclude(x => x.Player)
                .Include(x => x.ReplayDatas)
                .ThenInclude(x => x.ReplayFragments)
                .Include(x => x.ReplayDatas)
                .ThenInclude(x => x.ReplayChats)
                .Include(x => x.ReplayDatas)
                .ThenInclude(x => x.ReplayGoals)
                .Select(game => new GameDataViewModel
                {
                    Id = game.Id,
                    State = game.State.Name,
                    Date = game.CreatedOn,
                    RedScore = game.RedScore,
                    BlueScore = game.BlueScore,
                    ReplayId = game.ReplayDatas.Any() ? game.ReplayDatas.FirstOrDefault().Id : null,
                    HasReplayFragments = game.ReplayDatas.Any() ? game.ReplayDatas.FirstOrDefault().ReplayFragments.Count != 0 : false,
                    ChatMessages = new List<ReplayChat>(),
                    Goals = game.ReplayDatas.Any() ? game.ReplayDatas.FirstOrDefault().ReplayGoals.OrderBy(x => x.Packet).ToList() : new List<ReplayGoal>(),
                    ReplayUrl = storageUrl + "replays/" + game.Id.ToString() + ".hrp",
                    Players = game.GamePlayers.Select(x => new GameDataPlayerViewModel
                    {
                        Id = x.PlayerId,
                        Name = x.Player.Name,
                        Goals = x.Goals,
                        Assists = x.Assists,
                        Score = x.Score,
                        Team = x.Team,
                        Shots = x.Shots,
                        Conceded = x.Conceded,
                        Possession = x.Possession,
                        Saves = x.Saves
                    }).ToList()
                }).FirstOrDefaultAsync(x => x.Id == request.Id);

            return result;
        }

        public async Task<int> GetPlayerElo(int id)
        {
            var currentSeason = await GetCurrentSeason();

            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");
            var startingElo = _dbContext.Settings.FirstOrDefault().StartingElo;

            var sum = _dbContext.GamePlayers.Include(x => x.Player).Include(x=>x.Game).ThenInclude(x=>x.Season).Where(x => x.Player.Id == id && x.Game.Season == currentSeason && (x.Game.State == ended || x.Game.State == resigned)).Sum(x => x.Score);
            var eloOnSeasonStart = await _dbContext.Elos.Include(x=>x.Player).FirstOrDefaultAsync(x=>x.Player.Id == id && x.Season == currentSeason);
            return sum + (eloOnSeasonStart != null ? eloOnSeasonStart.Value : startingElo);
        }

        public async Task<RulesViewModel> GetRules()
        {
            var rules = new RulesViewModel();

            var setting = await _dbContext.Settings.FirstOrDefaultAsync();
            if (setting != null)
            {
                rules.Text = setting.Rules;
                rules.Rules = await _dbContext.Rules.Select(x => new RulesItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                }).ToListAsync();
            }

            return rules;
        }

        public async Task<string> GetStorage()
        {
            var storageUrl = String.Empty;

            var setting = await _dbContext.Settings.FirstOrDefaultAsync();
            if (setting != null)
            {
                storageUrl = String.Format("https://{0}/{1}/{2}/", setting.S3Domain, setting.S3Bucket, setting.Id);
            }

            return storageUrl;
        }

        public async Task<List<TopStatsViewModel>> GetTopStats()
        {
            var result = new List<TopStatsViewModel>();

            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");

            result = await _dbContext.Players
                .Include(x=>x.Cost)
                .Include(x => x.GamePlayers)
                .ThenInclude(x => x.Game)
                .Where(x => x.GamePlayers.Count > 50)
                .Select(x => new TopStatsViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Gp = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Count(),
                    Goals = x.GamePlayers.Where(x=>x.Game.State == ended || x.Game.State == resigned).Sum(gp=>gp.Goals),
                    Assists = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Sum(gp => gp.Assists),
                    Wins = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Count(gp => (gp.Team == 0 && gp.Game.RedScore > gp.Game.BlueScore) || (gp.Team == 1 && gp.Game.RedScore < gp.Game.BlueScore)),
                    Loses = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Count(gp => (gp.Team == 1 && gp.Game.RedScore > gp.Game.BlueScore) || (gp.Team == 0 && gp.Game.RedScore < gp.Game.BlueScore)),
                    GoalsPerGame = Math.Round(x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Sum(gp => gp.Goals) / (double)x.GamePlayers.Count, 2),
                    AssistsPerGame = Math.Round(x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Sum(gp => gp.Assists) / (double)x.GamePlayers.Count, 2),
                    Winrate = Math.Round(x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Count(gp => (gp.Team == 0 && gp.Game.RedScore > gp.Game.BlueScore) || (gp.Team == 1 && gp.Game.RedScore < gp.Game.BlueScore)) / (double)x.GamePlayers.Count * 100, 2),
                    Elo = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Sum(gp=>gp.Score),
                    Cost = x.Cost!=null? x.Cost.Cost: 0
                })
                .OrderByDescending(x=>x.Elo)
                .ToListAsync();

            return result;
        }

        public async Task<List<AdminStoryViewModel>> GetMainStories()
        {
            var dateDayBefore = DateTime.UtcNow.AddDays(-1);

            var result = await _dbContext.AdminStories.Where(x => (x.Expiration && x.CreatedOn > dateDayBefore) || !x.Expiration).OrderBy(x => x.CreatedOn).Select(x => new AdminStoryViewModel
            {
                Id = x.Id,
                Date = x.CreatedOn,
                Text = x.Text,
                Expiration = x.Expiration,
                Link = x.Link
            }).ToListAsync();
            return result;
        }

        public async Task<string> Report(Guid gameId, int toId, Guid reasonId, int tick, int fromId)
        {
            var result = String.Empty;

            var gamesCount = await _dbContext.GamePlayers.Where(x => x.PlayerId == fromId).CountAsync();
            if (gamesCount < 20)
            {
                result = "You can't submit a report until you have played 20 games";
            }
            else
            {
                var weekBefore = DateTime.UtcNow.AddDays(-7);
                var reportedBefore = await _dbContext.Reports.Include(x => x.From).Include(x => x.To).Where(x => x.From.Id == fromId && x.To.Id == toId && x.CreatedOn > weekBefore).ToListAsync();
                if (reportedBefore.Any())
                {
                    result = "You can report this player once per week";
                }
                else
                {
                    var from = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == fromId);
                    var to = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == toId);
                    var reason = await _dbContext.Rules.FirstOrDefaultAsync(x => x.Id == reasonId);
                    var game = await _dbContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);

                    var report = _dbContext.Reports.Add(new Reports
                    {
                        From = from,
                        To = to,
                        Reason = reason,
                        Tick = tick,
                        Game = game
                    });

                    _dbContext.PatrolDecisions.Add(new PatrolDecision
                    {
                        From = from,
                        IsReported = true,
                        Report = report.Entity,
                    });

                    await _dbContext.SaveChangesAsync();
                }
            }

            return result;
        }

        public async Task ReportDecision(Guid id, int userId, bool isReported)
        {
            var from = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
            var report = await _dbContext.Reports.Include(x=>x.To).Include(x=>x.Reason).Include(x=>x.Game).FirstOrDefaultAsync(x => x.Id == id);

            if (isReported)
            {
                var weekBefore = DateTime.UtcNow.AddDays(-7);
                var reportedBefore = await _dbContext.Reports.Include(x => x.From).Include(x => x.To).Where(x => x.From.Id == from.Id && x.To.Id == report.To.Id && x.CreatedOn > weekBefore).ToListAsync();
                if (!reportedBefore.Any())
                {
                    _dbContext.Reports.Add(new Reports
                    {
                        From = from,
                        To = report.To,
                        Reason = report.Reason,
                        Tick = report.Tick,
                        Game = report.Game
                    });
                }
            }

            _dbContext.PatrolDecisions.Add(new PatrolDecision
            {
                From = from,
                IsReported = isReported,
                Report = report,
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PartolViewModel>> GetPatrol(int userId)
        {
            var weekBefore = DateTime.UtcNow.AddDays(-7);

            var userPatrolDecisions = await _dbContext.PatrolDecisions.Where(x => x.From.Id == userId).Select(x => x.Report.Id).ToListAsync();
            var gamesCount = await _dbContext.GamePlayers.Where(x => x.PlayerId == userId).CountAsync();
            if (gamesCount < 20)
            {
                return new List<PartolViewModel>();
            }

            var player = await _dbContext.Players.Include(x => x.Bans).Include(x => x.NicknameChanges).SingleOrDefaultAsync(x => x.Id == userId);
            if (player != null)
            {
                var lastBan = player.Bans.Where(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                if (lastBan != null)
                {
                    return new List<PartolViewModel>();
                }
            }

            var patrols = await _dbContext.Reports
                .Include(x => x.From)
                .Include(x => x.To)
                .Include(x => x.Reason)
                .Where(x => x.From.Id != userId && x.To.Id != userId && !userPatrolDecisions.Contains(x.Id) && x.CreatedOn > weekBefore)
                .Select(x => new PartolViewModel
                {
                    ReportId = x.Id,
                    Reason = x.Reason.Title,
                    Date = x.CreatedOn
                })
                .ToListAsync();

            return patrols;
        }
    }
}
