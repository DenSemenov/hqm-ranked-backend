using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class TeamsService: ITeamsService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        public TeamsService(RankedDb dbContext, ISeasonService seasonService)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
        }
        public async Task<TeamsStateViewModel> GetTeamsState(int? userId)
        {
            var result = new TeamsStateViewModel();

            if (userId != null)
            {
                var currentSeason = await _seasonService.GetCurrentSeason();
                var player = await _dbContext.Players.Include(x=>x.Cost).FirstOrDefaultAsync(x=>x.Id == userId);
                if (player != null)
                {
                    var teamPlayer = await _dbContext.TeamPlayers.Include(x => x.Team).FirstOrDefaultAsync(x => x.Player == player);

                    if (teamPlayer != null)
                    {
                        result.IsCaptain = teamPlayer.Team.Captain == player;
                        result.IsAssistant = teamPlayer.Team.Assistant == player;
                        result.Team = new CurrentTeamViewModel
                        {
                            Id = teamPlayer.Team.Id,
                            Name = teamPlayer.Team.Name,
                        };
                    }
                    else
                    {
                        result.CanCreateTeam = player.Cost != null;
                    }
                }
            }

            return result;
        }

        public async Task CreateTeam(string name, int userId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
            if (state.CanCreateTeam && player != null)
            {
                var currentSeason = await _seasonService.GetCurrentSeason();

               var teamEntity=  _dbContext.Teams.Add(new Team
                {
                    Name = name,
                    Season = currentSeason,
                    Captain = player,
                });

                _dbContext.TeamPlayers.Add(new TeamPlayer
                {
                    Player = player,
                    Team = teamEntity.Entity,
                });

                _dbContext.Budgets.Add(new Budget
                {
                     Team = teamEntity.Entity,
                     Type = BudgetType.Start,
                     Change = 2500000,
                });

                _dbContext.Budgets.Add(new Budget
                {
                    Team = teamEntity.Entity,
                    Type = BudgetType.Invite,
                    Change = -player.Cost.Cost,
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
