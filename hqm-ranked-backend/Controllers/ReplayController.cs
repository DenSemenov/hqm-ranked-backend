using hqm_ranked_backend.Common;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReplayHandler.Classes;
using Serilog;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReplayController : ControllerBase
    {
        IReplayService _replayService;
        IReplayCalcService _replayCalcService;
        public ReplayController(IReplayService replayService, IReplayCalcService replayCalcService)
        {
            _replayService = replayService;
            _replayCalcService = replayCalcService;
        }

        [HttpPost("ProcessHrpLocal")]
        public async Task<IActionResult> ProcessHrpLocal([FromForm] IFormFile file)
        {

            long length = file.Length;
            if (length < 0)
                return BadRequest();

            using var fileStream = file.OpenReadStream();
            byte[] bytes = new byte[length];
            fileStream.Read(bytes, 0, (int)file.Length);

            var result = await _replayCalcService.ProcessReplay(bytes);

            return Ok(result);
        }

        [HttpPost("ProcessHrp")]
        public async Task ProcessHrpAsync([FromForm] Guid gameId, [FromForm] string token, [FromForm] IFormFile replay)
        {
            Log.Information(LogHelper.GetInfoLog("Replay gameId: " + gameId));
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
        [HttpPost("GetReplayHighlights")]
        public async Task<IActionResult> GetReplayHighlights(ReplayRequest request)
        {
            var result = await _replayService.GetReplayHighlights(request);

            return Ok(result);
        }

        [HttpPost("GetReplayStories")]
        public async Task<IActionResult> GetReplayStories()
        {
            var result = await _replayService.GetReplayStories();

            return Ok(result);
        }

        [HttpPost("GetStoryReplayViewer")]
        public async Task<IActionResult> GetStoryReplayViewer(StoryReplayViewerRequest request)
        {
            var result = await _replayService.GetStoryReplayViewer(request);

            return Ok(result);
        }

        [HttpPost("GetReportViewer")]
        public async Task<IActionResult> GetReportViewer(ReportViewerRequest request)
        {
            var result = await _replayService.GetReportViewer(request);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("LikeStory")]
        public async Task<IActionResult> LikeStory(StoryLikeRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _replayService.LikeStory(request.Id, userId);

            return Ok();
        }
    }
}
