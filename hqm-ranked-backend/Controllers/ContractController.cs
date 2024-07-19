using hqm_ranked_backend.Common;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_models.InputModels;
using hqm_ranked_services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        IContractService _contractService;
        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [HttpPost("GetContracts")]
        public async Task<IActionResult> GetContracts()
        {
            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = UserHelper.GetUserId(User);
            }

            var result = await _contractService.GetContracts(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("SelectContract")]
        public async Task<IActionResult> SelectContract(SelectContractRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _contractService.SelectContract(request, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("GetCoins")]
        public async Task<IActionResult> GetCoins()
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _contractService.GetCoins(userId);
            return Ok(result);
        }
    }
}
