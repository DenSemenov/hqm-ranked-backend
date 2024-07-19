using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_models.DTO;
using hqm_ranked_models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

            await GetCurrentSeason();

            return result;
        }

        public async Task<Season> GetCurrentSeason()
        {
            var result = await _dbContext.Seasons.FirstOrDefaultAsync(x => x.DateStart <= DateTime.UtcNow && x.DateEnd > DateTime.UtcNow);

            if (result == null)
            {
                result = await CreateNewSeason();
            }

            return result;
        }

        public async Task<Season> CreateNewSeason()
        {
            var currentDate = DateTime.UtcNow;
            var newSeason = new Season
            {
                DateStart = currentDate,
                DateEnd = currentDate.AddMonths(3),
                Name = "Season " + (_dbContext.Seasons.Count() + 1),
            };

            var entity= _dbContext.Seasons.Add(newSeason);

            var prevSeason = await _dbContext.Seasons.Where(x => x.DateEnd < DateTime.UtcNow).OrderByDescending(x => x.DateEnd).FirstOrDefaultAsync();

            if (prevSeason != null)
            {
                var prevSeasonData = await GetSeasonStats(new CurrentSeasonStatsRequest
                {
                    Offset = 0,
                    SeasonId = prevSeason.Id
                });

                var startElo = _dbContext.Settings.FirstOrDefault().StartingElo;

                foreach (var playerPrevSeason in prevSeasonData)
                {
                    var newElo = (int)(startElo + (playerPrevSeason.Rating - startElo) * 0.5);
                    var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == playerPrevSeason.PlayerId);
                    _dbContext.Elos.Add(new Elo
                    {
                        Season = entity.Entity,
                        Player = player,
                        Value = newElo
                    });
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }

            return newSeason;
        }

        public async Task<List<SeasonStatsViewModel>> GetSeasonStats(CurrentSeasonStatsRequest request)
        {
            var result = new List<SeasonStatsViewModel>();

            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");
            var startingElo = _dbContext.Settings.FirstOrDefault().StartingElo;

            var season = await _dbContext.Seasons.SingleOrDefaultAsync(x => x.Id == request.SeasonId);

            var players = _dbContext.GamePlayers
                .Include(x => x.Game)
                .Include(x => x.Player)
                .Where(x => x.Game.Season == season && (x.Game.State == ended || x.Game.State == resigned) && x.Game.InstanceType == Common.InstanceType.Ranked)
                .Select(x => new
                {
                    PlayerId = x.PlayerId,
                    Nickname = x.Player.Name,
                    Goals = x.Goals,
                    Assists = x.Assists,
                    Win = x.Team == 0 ? x.Game.RedScore > x.Game.BlueScore : x.Game.RedScore < x.Game.BlueScore,
                    Lose = x.Team == 0 ? x.Game.RedScore < x.Game.BlueScore : x.Game.RedScore > x.Game.BlueScore,
                    Mvp = x.Game.MvpId == x.PlayerId,
                    Score = x.Score
                })
                .GroupBy(x => x.PlayerId)
                .ToList();

            var dateWeekAgo = DateTime.UtcNow.AddDays(-7);
            if (request.DateAgo !=null)
            {
                dateWeekAgo = (DateTime)request.DateAgo;
            }

            var playersWeekAgo = _dbContext.GamePlayers
                .Include(x => x.Game)
                .Where(x => x.Game.Season == season && (x.Game.State == ended || x.Game.State == resigned) && x.Game.InstanceType == Common.InstanceType.Ranked && x.CreatedOn < dateWeekAgo)
                .Select(x => new
                {
                    PlayerId = x.PlayerId,
                    Score = x.Score
                })
                .GroupBy(x => x.PlayerId)
                .ToList();

            var elos = await _dbContext.Elos.Include(x => x.Player).Where(x => x.Season == season).ToListAsync();

            foreach (var player in players)
            {
                var elo = startingElo;

                var playerElo = elos.SingleOrDefault(x => x.Player.Id == player.Key && x.Season == season);
                if (playerElo != null)
                {
                    elo = playerElo.Value;
                }

                var oldRating = elo;

                var playerWeekAgo = playersWeekAgo.FirstOrDefault(x => x.Key == player.Key);
                if (playerWeekAgo != null)
                {
                    oldRating += playerWeekAgo.Sum(x => x.Score);
                }

                result.Add(new SeasonStatsViewModel
                {
                    PlayerId = player.Key,
                    Nickname = player.FirstOrDefault().Nickname,
                    Goals = player.Sum(x => x.Goals),
                    Assists = player.Sum(x => x.Assists),
                    Win = player.Count(x => x.Win),
                    Lose = player.Count(x => x.Lose),
                    Mvp = player.Count(x => x.Mvp),
                    Rating = player.Sum(x => x.Score) + elo,
                    RatingWeekAgo = oldRating,
                    Change = 0
                });
            }

            result = result.OrderByDescending(x => x.Rating).ToList();

            var i = 1;
            foreach (var player in result)
            {
                player.Place = i;
                i++;
            }

            i = 1;
            foreach (var player in result.OrderByDescending(x => x.RatingWeekAgo))
            {
                player.PlaceWeekAgo = i;
                i++;
            }

            foreach (var player in result)
            {
                player.Change = player.PlaceWeekAgo - player.Place;
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
                .Include(x => x.RedTeam)
                .Include(x => x.BlueTeam)
                .Include(x=>x.GameInvites)
                .Include(x => x.ReplayDatas)
                .ThenInclude(x => x.ReplayFragments)
                .Where(x => x.Season == season && x.InstanceType == Common.InstanceType.Ranked)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new SeasonGameViewModel
                {
                    GameId = x.Id,
                    Date = x.GameInvites.Any() ? x.GameInvites.FirstOrDefault().Date :(x.LastModifiedOn ?? x.CreatedOn),
                    RedScore = x.RedScore,
                    BlueScore = x.BlueScore,
                    Status = x.State.Name,
                    ReplayId = x.ReplayDatas.Any() ? x.ReplayDatas.FirstOrDefault().Id : null,
                    HasReplayFragments = x.ReplayDatas.Any() ? x.ReplayDatas.FirstOrDefault().ReplayFragments.Count != 0 : false,
                    InstanceType = x.InstanceType,
                    RedTeamId = x.RedTeamId,
                    BlueTeamId = x.BlueTeamId,
                    RedTeamName = x.RedTeam !=null? x.RedTeam.Name: String.Empty,
                    BlueTeamName = x.BlueTeam != null ? x.BlueTeam.Name : String.Empty,
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

            var teamsGames = await _dbContext.Games
               .Include(x => x.State)
               .Include(x => x.GamePlayers)
               .ThenInclude(x => x.Player)
               .Include(x => x.RedTeam)
               .Include(x => x.BlueTeam)
               .Include(x => x.GameInvites)
               .Include(x => x.ReplayDatas)
               .ThenInclude(x => x.ReplayFragments)
               .Where(x => x.Season == season && x.InstanceType == Common.InstanceType.Teams)
               .OrderByDescending(x => x.CreatedOn)
               .Select(x => new SeasonGameViewModel
               {
                   GameId = x.Id,
                   Date = x.GameInvites.Any() ? x.GameInvites.FirstOrDefault().Date : (x.LastModifiedOn ?? x.CreatedOn),
                   RedScore = x.RedScore,
                   BlueScore = x.BlueScore,
                   Status = x.State.Name,
                   ReplayId = x.ReplayDatas.Any() ? x.ReplayDatas.FirstOrDefault().Id : null,
                   HasReplayFragments = x.ReplayDatas.Any() ? x.ReplayDatas.FirstOrDefault().ReplayFragments.Count != 0 : false,
                   InstanceType = x.InstanceType,
                   RedTeamId = x.RedTeamId,
                   BlueTeamId = x.BlueTeamId,
                   RedTeamName = x.RedTeam != null ? x.RedTeam.Name : String.Empty,
                   BlueTeamName = x.BlueTeam != null ? x.BlueTeam.Name : String.Empty,
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
            games.AddRange(teamsGames);
            return games;
        }

        public async Task<PlayerViewModel> GetPlayerData(PlayerRequest request)
        {
            var result = new PlayerViewModel();

            result.Id = request.Id;

            var player = await _dbContext.Players
                .Include(x=>x.PlayerCalcStats)
                .Include(x => x.Awards).ThenInclude(x => x.Season)
                .Include(x => x.GamePlayers).ThenInclude(x => x.Game).ThenInclude(x => x.Season)
                .Include(x => x.GamePlayers).ThenInclude(x => x.Game).ThenInclude(x => x.RedTeam)
                .Include(x => x.GamePlayers).ThenInclude(x => x.Game).ThenInclude(x => x.BlueTeam)
                .Include(x => x.GamePlayers).ThenInclude(x => x.Game).ThenInclude(x => x.GamePlayers).ThenInclude(x => x.Player)
                .Include(x => x.NicknameChanges).Include(x => x.Cost).Select(x =>
            new
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.GamePlayers.Count,
                Goals = x.GamePlayers.Sum(x => x.Goals),
                Assists = x.GamePlayers.Sum(x => x.Assists),
                LastGames = x.GamePlayers.OrderByDescending(x => x.Game.CreatedOn).Take(3).Select(x=> new PlayerLastGamesViewModel
                {
                    Date = x.CreatedOn,
                    GameId = x.Game.Id,
                    Goals = x.Goals,
                    Assists = x.Assists,
                    Score = x.Score,
                    RedScore = x.Game.RedScore,
                    BlueScore = x.Game.BlueScore,
                    InstanceType = x.Game.InstanceType,
                    RedTeamId = x.Game.RedTeamId,
                    BlueTeamId = x.Game.BlueTeamId,
                    RedTeamName = x.Game.RedTeam != null ? x.Game.RedTeam.Name : String.Empty,
                    BlueTeamName = x.Game.BlueTeam != null ? x.Game.BlueTeam.Name : String.Empty,
                    Players = x.Game.GamePlayers.Select(x => new GameDataPlayerViewModel
                    {
                        Id = x.PlayerId,
                        Name = x.Player.Name,
                        Goals = x.Goals,
                        Assists = x.Assists,
                        Score = x.Score,
                        Team = x.Team,
                    }).ToList()
                }).ToList(),
                LastPoints = x.GamePlayers.OrderByDescending(x => x.Game.CreatedOn).Take(50).Select(x => new
                {
                    SeasonId = x.Game.Season.Id,
                    Elo = x.Score
                }),
                NicknameChanges = x.NicknameChanges.OrderByDescending(x => x.CreatedOn).Select(x => x.OldNickname).ToList(),
                Cost = x.Cost != null ? x.Cost.Cost : 0,
                Awards = x.Awards.Select(y => new PlayerAwardViewModel
                {
                    AwardType = y.AwardType,
                    Date = y.CreatedOn,
                    Count = y.Count,
                    SeasonName = y.Season != null ? y.Season.Name : String.Empty
                }).ToList(),
                PlayerCalcStats = x.PlayerCalcStats
            }).SingleOrDefaultAsync(x => x.Id == request.Id);

            result.Name = player.Name;
            result.Games = player.Count;
            result.Goals = player.Goals;
            result.Assists = player.Assists;
            result.Points = result.Goals + result.Assists;
            result.Cost = player.Cost;

            result.CalcStats = new PlayerCalcStatsViewModel
            {
                Mvp = player.PlayerCalcStats != null ? Math.Round(player.PlayerCalcStats.Mvp,2) : 0,
                Goals = player.PlayerCalcStats != null ? Math.Round(player.PlayerCalcStats.Goals, 2) : 0,
                Assists = player.PlayerCalcStats != null ? Math.Round(player.PlayerCalcStats.Assists, 2) : 0,
                Winrate = player.PlayerCalcStats != null ? Math.Round(player.PlayerCalcStats.Winrate, 2) : 0,
                Shots = player.PlayerCalcStats != null ? Math.Round(player.PlayerCalcStats.Shots, 2) : 0,
                Saves = player.PlayerCalcStats != null ? Math.Round(player.PlayerCalcStats.Saves, 2) : 0,
            };

            result.Awards = player.Awards.OrderByDescending(x=>x.SeasonName).ToList();

            var includedSeasons = player.LastPoints.Select(x=>x.SeasonId).Distinct().ToList();
            var elosPerSeasons = new List<SeasonEloModel>();

            foreach (var season in includedSeasons)
            {
                elosPerSeasons.Add(new SeasonEloModel
                {
                    SeasonId = season,
                    Elo = await GetPlayerEloBySeason(player.Id, season)
                });
            }

            foreach (var season in elosPerSeasons)
            {
                var endElo = season.Elo;

                foreach(var point in player.LastPoints.Where(x=>x.SeasonId == season.SeasonId))
                {
                    result.PlayerPoints.Add(endElo);

                    endElo -= point.Elo;
                }
            }

            result.PlayerPoints.Reverse();

            result.LastGames = player.LastGames;

            result.CalcData = new PlayerCalcDataViewModel();

            result.OldNicknames = player.NicknameChanges;

            return result;
        }

        public async Task<PlayerLiteDataViewModel> GetPlayerLiteData(PlayerRequest request)
        {
            var player = await _dbContext.Players
                .Include(x => x.PlayerCalcStats)
                .Include(x => x.GamePlayers)
                .Select(x =>
            new PlayerLiteDataViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Gp = x.GamePlayers.Count,
                Goals = x.GamePlayers.Sum(x => x.Goals),
                Assists = x.GamePlayers.Sum(x => x.Assists),
                CalcStats = new PlayerCalcStatsViewModel
                {
                    Mvp = x.PlayerCalcStats != null ? Math.Round(x.PlayerCalcStats.Mvp, 2) : 0,
                    Goals = x.PlayerCalcStats != null ? Math.Round(x.PlayerCalcStats.Goals, 2) : 0,
                    Assists = x.PlayerCalcStats != null ? Math.Round(x.PlayerCalcStats.Assists, 2) : 0,
                    Winrate = x.PlayerCalcStats != null ? Math.Round(x.PlayerCalcStats.Winrate, 2) : 0,
                    Shots = x.PlayerCalcStats != null ? Math.Round(x.PlayerCalcStats.Shots, 2) : 0,
                    Saves = x.PlayerCalcStats != null ? Math.Round(x.PlayerCalcStats.Saves, 2) : 0,
                }
            }).SingleOrDefaultAsync(x => x.Id == request.Id);

            return player;
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
                .Include(x => x.RedTeam)
                .Include(x => x.BlueTeam)
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
                    InstanceType = game.InstanceType,
                    RedTeamId = game.RedTeamId,
                    BlueTeamId = game.BlueTeamId,
                    RedTeamName = game.RedTeam != null ? game.RedTeam.Name : String.Empty,
                    BlueTeamName = game.BlueTeam != null ? game.BlueTeam.Name : String.Empty,
                    RedPoints = game.RedPoints ?? 0,
                    BluePoints = game.BluePoints ?? 0,
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

            var sum = _dbContext.GamePlayers.Include(x => x.Player).Include(x=>x.Game).ThenInclude(x=>x.Season).Where(x => x.Player.Id == id && x.Game.Season == currentSeason && x.Game.InstanceType == Common.InstanceType.Ranked && (x.Game.State == ended || x.Game.State == resigned)).Sum(x => x.Score);
            var eloOnSeasonStart = await _dbContext.Elos.Include(x=>x.Player).FirstOrDefaultAsync(x=>x.Player.Id == id && x.Season == currentSeason);
            return sum + (eloOnSeasonStart != null ? eloOnSeasonStart.Value : startingElo);
        }

        public async Task<int> GetPlayerEloBySeason(int id, Guid seasonId)
        {
            var currentSeason = await _dbContext.Seasons.FirstOrDefaultAsync(x => x.Id == seasonId);

            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");
            var startingElo = _dbContext.Settings.FirstOrDefault().StartingElo;

            var sum = _dbContext.GamePlayers.Include(x => x.Player).Include(x => x.Game).ThenInclude(x => x.Season).Where(x => x.Player.Id == id && x.Game.Season == currentSeason && x.Game.InstanceType == Common.InstanceType.Ranked && (x.Game.State == ended || x.Game.State == resigned)).Sum(x => x.Score);
            var eloOnSeasonStart = await _dbContext.Elos.Include(x => x.Player).FirstOrDefaultAsync(x => x.Player.Id == id && x.Season == currentSeason);
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

        public async Task<HomeStatsViewModel> GetHomeStats()
        {
            var result = new HomeStatsViewModel();
            var total = 500;
            var dates = await _dbContext.Games.OrderByDescending(x => x.CreatedOn).Take(total).Select(x => x.CreatedOn).ToListAsync();

            for (int i = 0; i <= 23; i++)
            {
                result.Daily.Add(new HomeStatsDailyViewModel
                {
                    Hour = i,
                    Count = dates.Where(x => x.Hour == i).Count()/ (double)total* 100
                });
            }

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                result.Weekly.Add(new HomeStatsWeeklyViewModel
                {
                    Day = day.ToString(),
                    Count = dates.Where(x => x.DayOfWeek == day).Count() / (double)total * 100
                });
            }

            return result;
        }
    }
}
