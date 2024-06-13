using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using hqm_ranked_backend.Models.DbModels;
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
                })
                .ToListAsync();

            var min = result.Min(x=>x.Winrate);
            var max = result.Max(x => x.Winrate);
            var diff = max - min;
            var calculated = result.Select(x => new
            {
                Id = x.Id,
                Value = (int)(900000 * (double)(x.Winrate - min)/diff) + 100000
            }).ToList();

            foreach(var item in calculated)
            {
                var player = await _dbContext.Players.Include(x=>x.Cost).FirstOrDefaultAsync(x => x.Id == item.Id);
                if (player != null)
                {
                    if (player.Cost != null)
                    {
                        player.Cost.Cost = item.Value;
                    }
                    else
                    {
                        player.Cost = new PlayerCost
                        {
                             Cost = item.Value,
                        };
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
