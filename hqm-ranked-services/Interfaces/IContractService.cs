using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_services.Interfaces
{
    public interface IContractService
    {
        Task<List<ContractViewModel>> GetContracts(int? userId);
        Task SelectContract(SelectContractRequest request, int userId);
        Task<int> GetCoins(int userId);
    }
}
