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
                using (var ms = new MemoryStream())
                {
                    replay.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    await _replayService.PushReplay(gameId,fileBytes, token);
                }
            }
        }

        [HttpPost("GetReplayData")]
        public async Task<IActionResult> GetReplayData(ReplayRequest request)
        {
            var result = await _replayService.GetReplayData(request);

            return Ok(result);
        }
    }
}
