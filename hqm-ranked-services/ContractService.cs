using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_helpers;
using hqm_ranked_models.ViewModels;
using hqm_ranked_services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_services
{
    public class ContractService: IContractService
    {
        private RankedDb _dbContext;
        public ContractService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ContractViewModel>> GetContracts(int? userId)
        {
            var canSelect = false;

            if (userId != null)
            {
                var weekStartDate = DateHelper.StartOfWeek(DateTime.UtcNow, DayOfWeek.Monday);
                var countSelected = await _dbContext.ContractSelects.Include(x => x.Player).Where(x => x.Player.Id == userId && x.CreatedOn > weekStartDate).CountAsync();
                if (countSelected < 2)
                {
                    canSelect = true;
                }
            }

            var result = await _dbContext.Contracts.OrderBy(x => x.ContractType).ThenBy(x=>x.Count).Select(x => new ContractViewModel
            {
                CanSelect = canSelect,
                ContractType = x.ContractType,
                Count = x.Count,
                Points = x.Points,
            }).ToListAsync();

            return result;
        }
    }
}
