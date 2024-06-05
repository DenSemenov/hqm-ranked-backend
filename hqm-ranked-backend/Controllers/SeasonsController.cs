using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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


        [HttpPost("GetSeasonGames")]
        public async Task<IActionResult> GetSeasonLastGames(CurrentSeasonStatsRequest request)
        {
            var result = await _seasonService.GetSeasonGames(request);

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

        [HttpPost("GetRules")]
        public async Task<IActionResult> GetRules()
        {
            var result = await _seasonService.GetRules();

            return Ok(result);
        }

        [HttpPost("GetStorage")]
        public async Task<IActionResult> GetStorage()
        {
            var result = await _seasonService.GetStorage();

            return Ok(result);
        }

        [HttpPost("GetTopStats")]
        public async Task<IActionResult> GetTopStats()
        {
            var result = await _seasonService.GetTopStats();

            return Ok(result);
        }

        [HttpPost("GetMainStories")]
        public async Task<IActionResult> GetMainStories()
        {
            var result = await _seasonService.GetMainStories();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Report")]
        public async Task<IActionResult> Report(PlayerReportRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _seasonService.Report(request.GameId, request.Id, request.ReasonId, request.Tick, userId);

            return Ok();
        }
    }
}
