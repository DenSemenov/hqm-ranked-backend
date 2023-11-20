using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
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

        public async Task<List<SeasonViewModel>> GetSeasons(CurrentDivisionRequest request)
        {
            var result = await _dbContext.Seasons.Where(x=>x.DivisionId == request.DivisionId).Select(x => new SeasonViewModel
            {
                Id = x.Id,
                Name = x.Name,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd,
                DivisionId = x.DivisionId,
            }).OrderByDescending(x=>x.DateStart).ToListAsync();

            return result;
        }

        public async Task<List<Division>> GetDivisions()
        {
            return await _dbContext.Divisions.ToListAsync();
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

            foreach(var game in games)
            {
                result.Add(new SeasonStatsViewModel
                {
                    PlayerId = game.Key,
                    Nickname = game.FirstOrDefault().Nickname,
                    Goals = game.Sum(x => x.Goals),
                    Assists = game.Sum(x => x.Assists),
                    Win = game.Count(x => x.Win),
                    Lose = game.Count(x => x.Lose),
                    Mvp = game.Count(x => x.Mvp),
                    Rating = game.Sum(x => x.Score)
                });
            }

            result = result.OrderByDescending(x => x.Rating).ToList();

            return result;
        }

        public async Task<List<SeasonGameViewModel>> GetSeasonLastGames(CurrentSeasonStatsRequest request)
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
                    TeamNameRed = x.GamePlayers.Any(x => x.Team == 0) ? x.GamePlayers.FirstOrDefault(x => x.Team == 0).Player.Name : "Red",
                    TeamNameBlue = x.GamePlayers.Any(x => x.Team == 1) ? x.GamePlayers.FirstOrDefault(x => x.Team == 1).Player.Name : "Blue",
                    TeamRedId = x.GamePlayers.Any(x => x.Team == 0) ? x.GamePlayers.FirstOrDefault(x => x.Team == 0).Player.Id : Guid.Empty,
                    TeamBlueId = x.GamePlayers.Any(x => x.Team == 1) ? x.GamePlayers.FirstOrDefault(x => x.Team == 1).Player.Id : Guid.Empty,
                })
                .Take(10)
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

            var currentSeasonStats = await GetSeasons(new CurrentDivisionRequest { DivisionId = request.DivisionId });
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
                GameId = x.Id,
                Goals = x.Goals,
                Assists = x.Assists,
                RedScore = x.Game.RedScore,
                BlueScore = x.Game.BlueScore,
                TeamRedName = x.Game.GamePlayers.Any(x => x.Team == 0) ? x.Game.GamePlayers.FirstOrDefault(x => x.Team == 0).Player.Name : "Red",
                TeamBlueName = x.Game.GamePlayers.Any(x => x.Team == 1) ? x.Game.GamePlayers.FirstOrDefault(x => x.Team == 1).Player.Name : "Blue",
                TeamRedId = x.Game.GamePlayers.Any(x => x.Team == 0) ? x.Game.GamePlayers.FirstOrDefault(x => x.Team == 0).Player.Id : Guid.Empty,
                TeamBlueId = x.Game.GamePlayers.Any(x => x.Team == 1) ? x.Game.GamePlayers.FirstOrDefault(x => x.Team == 1).Player.Id : Guid.Empty,
                Team= x.Team
            }).ToList();

            result.CalcData = new PlayerCalcDataViewModel();

            return result;
        }

    }
}
