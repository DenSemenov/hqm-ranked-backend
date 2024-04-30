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
        public void ProcessHrp([FromForm] string time, [FromForm] string server, [FromForm] IFormFile replay, string token)
        {
            if (replay.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    replay.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    _replayService.PushReplay(fileBytes, token);
                }
            }
        }
    }
}
