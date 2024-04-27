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


        [HttpPost("GetSeasonLastGames")]
        public async Task<IActionResult> GetSeasonLastGames(CurrentSeasonStatsRequest request)
        {
            var result = await _seasonService.GetSeasonLastGames(request);

            return Ok(result);
        }

        [HttpPost("GetPlayerData")]
        public async Task<IActionResult> GetPlayerData(PlayerRequest request)
        {
            var result = await _seasonService.GetPlayerData(request);

            return Ok(result);
        }

        [HttpPost("GetGameData")]
        public async Task<IActionResult> GetGameData(GameRequest request)
        {
            var result = await _seasonService.GetGameData(request);

            return Ok(result);
        }
    }
}
