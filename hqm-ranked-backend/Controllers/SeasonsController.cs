using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        ISeasonService _seasonService;

        public SeasonsController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        [HttpPost("GetSeasons")]
        public async Task<IActionResult> GetSeasons()
        {
            var result = await _seasonService.GetSeasons();

            return Ok(result);
        }

        [HttpPost("GetSeasonStats")]
        public async Task<IActionResult> GetSeasonStats(CurrentSeasonStatsRequest request)
        {
            var result = await _seasonService.GetSeasonStats(request);

            return Ok(result);
        }
    }
}
