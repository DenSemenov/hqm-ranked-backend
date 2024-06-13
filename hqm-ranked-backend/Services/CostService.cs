using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.DTO;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.X86;

namespace hqm_ranked_backend.Services
{
    public class CostService: ICostService
    {
        private RankedDb _dbContext;
        public CostService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CalcCosts()
        {
            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");

            var result = await _dbContext.Players
                .Include(x => x.GamePlayers)
                .ThenInclude(x => x.Game)
                .ThenInclude(x => x.State)
                .Where(x => x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Count() > 100)
                .Select(x => new
                {
                    Id = x.Id,
                    Winrate = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Take(100).Count(gp => (gp.Team == 0 && gp.Game.RedScore > gp.Game.BlueScore) || (gp.Team == 1 && gp.Game.RedScore < gp.Game.BlueScore)),
                    PointsPerGame = x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Take(100).Sum(gp => gp.Goals+gp.Assists) / (double)x.GamePlayers.Where(x => x.Game.State == ended || x.Game.State == resigned).Take(100).Count(),
                })
                .ToListAsync();


            var calc = new List<CalculatedCostStats>();
            foreach (var item in result)
            {
                calc.Add(new CalculatedCostStats
                {
                    Id = item.Id,
                    PointsPerGame = item.PointsPerGame,
                    Winrate = item.Winrate
                });
            }

            var minWinrate = calc.Min(x => x.Winrate);
            var maxWinrate = calc.Max(x => x.Winrate);
            var diffWinrate = maxWinrate - minWinrate;

            var minPointsPerGame = calc.Min(x => x.PointsPerGame);
            var maxPointsPerGame = calc.Max(x => x.PointsPerGame);
            var diffPointsPerGame = maxPointsPerGame - minPointsPerGame;

            foreach (var item in calc)
            {
                var winratePercent = (double)(item.Winrate - minWinrate) / diffWinrate;
                var pointsPerGamePercent = (double)(item.PointsPerGame - minPointsPerGame) / diffPointsPerGame;

                var avg = (winratePercent + pointsPerGamePercent) / 2;

                item.Cost = (int)(900000 * avg + 100000);
            }

            foreach (var item in calc)
            {
                var player = await _dbContext.Players.Include(x=>x.Cost).FirstOrDefaultAsync(x => x.Id == item.Id);
                if (player != null)
                {
                    if (player.Cost != null)
                    {
                        player.Cost.Cost = item.Cost;
                    }
                    else
                    {
                        player.Cost = new PlayerCost
                        {
                             Cost = item.Cost,
                        };
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
