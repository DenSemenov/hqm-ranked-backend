using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReplayController : ControllerBase
    {
        IReplayService _replayService;
        public ReplayController(IReplayService replayService)
        {
            _replayService = replayService;
        }

        [HttpPost("ProcessHrp")]
        public async Task ProcessHrpAsync([FromForm] Guid gameId, [FromForm] string token, [FromForm] IFormFile replay)
        {
            if (replay.Length > 0)
            {
                await _replayService.PushReplay(gameId, replay, token);
            }
        }

        [HttpPost("GetReplayViewer")]
        public async Task<IActionResult> GetReplayViewer(ReplayViewerRequest request)
        {
            var result = await _replayService.GetReplayViewer(request);

            return Ok(result);
        }

        [HttpPost("GetReplayGoals")]
        public async Task<IActionResult> GetReplayGoals(ReplayRequest request)
        {
            var result = await _replayService.GetReplayGoals(request);

            return Ok(result);
        }

        [HttpPost("GetReplayChatMessages")]
        public async Task<IActionResult> GetReplayChatMessages(ReplayRequest request)
        {
            var result = await _replayService.GetReplayChatMessages(request);

            return Ok(result);
        }
    }
}
