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
    }
}
