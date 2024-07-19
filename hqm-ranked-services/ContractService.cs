using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_helpers;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using hqm_ranked_services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace hqm_ranked_services
{
    public class ContractService: IContractService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        public ContractService(RankedDb dbContext, ISeasonService seasonService)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
        }

        public async Task<List<ContractViewModel>> GetContracts(int? userId)
        {
            var selectedContractIds = new List<ContractSelect>();

            if (userId != null)
            {
                var weekStartDate = DateHelper.StartOfWeek(DateTime.UtcNow, DayOfWeek.Monday);
                selectedContractIds = await _dbContext.ContractSelects.Include(x => x.Player).Where(x => x.Player.Id == userId && x.CreatedOn > weekStartDate).ToListAsync();
            }



            var r = await _dbContext.Contracts.OrderBy(x => x.ContractType).ThenBy(x => x.Count).ToListAsync();
                
            var result =   r.Select(x => new ContractViewModel
            {
                Id = x.Id,
                IsSelected = selectedContractIds.Any(y=>y.Contract.Id == x.Id),
                SelectedDate = selectedContractIds.Any(y => y.Contract.Id == x.Id)? selectedContractIds.FirstOrDefault(y => y.Contract.Id == x.Id).CreatedOn: null,
                ContractType = x.ContractType,
                Count = x.Count,
                Points = x.Points,
                IsHidden = selectedContractIds.Count >=2 && !selectedContractIds.Any(y => y.Contract.Id == x.Id)
            }).ToList();

            return result;
        }

        public async Task SelectContract(SelectContractRequest request, int userId)
        {
            var contract = await _dbContext.Contracts.FirstOrDefaultAsync(x => x.Id == request.Id);
            var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
            if (contract != null && player != null)
            {
                _dbContext.ContractSelects.Add(new hqm_ranked_database.DbModels.ContractSelect
                {
                    Contract = contract,
                    Player = player,
                    Passed = false,
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetCoins(int userId)
        {
            var coins = 0;
            var player = await _dbContext.Players.Include(x=>x.ContractSelects).ThenInclude(x=>x.Contract).FirstOrDefaultAsync(x => x.Id == userId);
            if (player != null)
            {
                coins = player.ContractSelects.Where(x => x.Passed).Sum(x => x.Contract.Points);
            }

            return coins;
        }


        public async Task CalcContracts()
        {
            var weekStartDate = DateHelper.StartOfWeek(DateTime.UtcNow, DayOfWeek.Monday);
            var contractsToCalc = await _dbContext.ContractSelects.Include(x => x.Player).Include(x=>x.Contract).Where(x => x.CreatedOn > weekStartDate && !x.Passed).ToListAsync();

            foreach(var contract in contractsToCalc)
            {
                var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
                var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");
                var gamePlayers = _dbContext.GamePlayers.Include(x=>x.Game).ThenInclude(x=>x.GamePlayers).Include(x => x.Game).ThenInclude(x => x.State).Where(x=>x.PlayerId == contract.Player.Id && x.CreatedOn>= contract.CreatedOn && (x.Game.State == ended || x.Game.State == resigned)).OrderBy(x=>x.CreatedOn).ToList();

                switch (contract.Contract.ContractType)
                {
                    case hqm_ranked_database.DbModels.ContractType.Assists:
                        if (gamePlayers.Sum(x=>x.Assists)>= contract.Contract.Count)
                        {
                            contract.Passed = true;
                        }
                        break;
                    case hqm_ranked_database.DbModels.ContractType.WinWith800Elo:
                        
                        break;
                    case hqm_ranked_database.DbModels.ContractType.Saves:
                        if (gamePlayers.Sum(x => x.Saves) >= contract.Contract.Count)
                        {
                            contract.Passed = true;
                        }
                        break;
                    case hqm_ranked_database.DbModels.ContractType.WinWith20Possesion:
                        if (gamePlayers.Count(x=>x.Possession !=0 && x.Possession <= x.Game.GamePlayers.Where(y=>y.Team == x.Team).Sum(x=>x.Possession)/5 && ((x.Team == 0 && x.Game.RedScore> x.Game.BlueScore) || (x.Team == 1 && x.Game.BlueScore > x.Game.RedScore))) >= contract.Contract.Count)
                        {
                            contract.Passed = true;
                        }
                        break;
                    case hqm_ranked_database.DbModels.ContractType.Winstreak:
                        var maxCount = 0;
                        var currentCount = 1;

                        foreach(var gamePlayer in gamePlayers)
                        {
                            if ((gamePlayer.Team == 0 && gamePlayer.Game.RedScore> gamePlayer.Game.BlueScore) || (gamePlayer.Team == 1 && gamePlayer.Game.RedScore < gamePlayer.Game.BlueScore))
                            {
                                currentCount++;
                            }
                            else
                            {
                                maxCount = Math.Max(maxCount, currentCount);
                                currentCount = 1;
                            }
                        }

                        maxCount = Math.Max(maxCount, currentCount);

                        if (maxCount >= contract.Contract.Count)
                        {
                            contract.Passed = true;
                        }

                        break;
                    case hqm_ranked_database.DbModels.ContractType.RiseInRanking:
                        var currentSeason = await _seasonService.GetCurrentSeason();
                        var stats = await _seasonService.GetSeasonStats(new hqm_ranked_backend.Models.InputModels.CurrentSeasonStatsRequest
                        {
                            SeasonId = currentSeason.Id,
                            Offset = 0,
                            DateAgo = contract.CreatedOn
                        });

                        var foundPlayer = stats.FirstOrDefault(x => x.PlayerId == contract.Player.Id);
                        if (foundPlayer != null)
                        {
                            if (foundPlayer.Place - foundPlayer.PlaceWeekAgo >= contract.Contract.Count) 
                            {
                                contract.Passed = true;
                            }
                        }
                        break;
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
