using hqm_ranked_backend.Common;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_services.Interfaces;
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
    }
}
