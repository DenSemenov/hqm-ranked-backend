using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

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

            var season = await _dbContext.Seasons.SingleOrDefaultAsync(x => x.Id == request.SeasonId);
            var games = _dbContext.GamePlayers
                .Include(x => x.Game)
                .Include(x => x.Player)
                .Where(x => x.Game.Season == season)
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
                var elo = 1000;

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

            return result;
        }

        public async Task<List<SeasonGameViewModel>> GetSeasonGames(CurrentSeasonStatsRequest request)
        {
            var season = await _dbContext.Seasons.SingleOrDefaultAsync(x => x.Id == request.SeasonId);
            var games = await _dbContext.Games
                .Include(x => x.State)
                .Include(x => x.GamePlayers)
                .ThenInclude(x => x.Player)
                .Where(x => x.Season == season)
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new SeasonGameViewModel
                {
                    GameId = x.Id,
                    Date = x.LastModifiedOn ?? x.CreatedOn,
                    RedScore = x.RedScore,
                    BlueScore = x.BlueScore,
                    Status = x.State.Name,
                    Players = x.GamePlayers.Select(x=>new GamePlayerItem
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

            var player = await _dbContext.Players.Include(x => x.GamePlayers).ThenInclude(x => x.Game).ThenInclude(x => x.GamePlayers).ThenInclude(x => x.Player).SingleOrDefaultAsync(x => x.Id == request.Id);
            result.Name = player.Name;
            result.Games = player.GamePlayers.Count;
            result.Goals = player.GamePlayers.Sum(x => x.Goals);
            result.Assists = player.GamePlayers.Sum(x => x.Assists);
            result.Points = result.Goals + result.Assists;

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

            result.LastGames = player.GamePlayers.OrderByDescending(x => x.Game.CreatedOn).Take(3).Select(x => new PlayerLastGamesViewModel
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

            var score = 0;
            foreach(var game in player.GamePlayers)
            {

                score += game.Score;
                result.PlayerPoints.Add(new PlayerPoint
                {
                    GameId = game.Game.Id,
                    Score = score
                });
            }

            return result;
        }

        public async Task<GameDataViewModel> GetGameData(GameRequest request)
        {
            var result = new GameDataViewModel();

            var game = await _dbContext.Games.Include(x=>x.State).Include(x=>x.GamePlayers).ThenInclude(x=>x.Player).FirstOrDefaultAsync(x=>x.Id == request.Id); 
            if (game != null)
            {
                Guid? replayId = _dbContext.ReplayData.Any(x => x.Game == game) ? _dbContext.ReplayData.FirstOrDefault(x => x.Game == game).Id : null;

                result.Id = game.Id;
                result.State = game.State.Name;
                result.Date = game.CreatedOn;
                result.RedScore = game.RedScore;
                result.BlueScore = game.BlueScore;
                result.ReplayId = replayId;
                result.Players = game.GamePlayers.Select(x => new GameDataPlayerViewModel
                {
                    Id = x.PlayerId,
                    Name = x.Player.Name,
                    Goals = x.Goals,
                    Assists = x.Assists,
                    Score = x.Score,
                    Team = x.Team,
                }).ToList();
            }

            return result;
        }

        public async Task<int> GetPlayerElo(int id)
        {
            var currentSeason = await GetCurrentSeason();

            var sum = _dbContext.GamePlayers.Include(x => x.Player).Include(x=>x.Game).ThenInclude(x=>x.Season).Where(x => x.Player.Id == id && x.Game.Season == currentSeason).Sum(x => x.Score);
            var eloOnSeasonStart = await _dbContext.Elos.Include(x=>x.Player).FirstOrDefaultAsync(x=>x.Player.Id == id && x.Season == currentSeason);
            return sum + (eloOnSeasonStart != null ? eloOnSeasonStart.Value : 1000);
        }

        public async Task<string> GetRules()
        {
            var rules = String.Empty;

            var setting = await _dbContext.Settings.FirstOrDefaultAsync();
            if (setting != null)
            {
                rules = setting.Rules;
            }

            return rules;
        }
    }
}
