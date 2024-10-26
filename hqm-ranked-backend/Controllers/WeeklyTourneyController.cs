using hqm_ranked_backend.Common;
using hqm_ranked_models.InputModels;
using hqm_ranked_services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeeklyTourneyController : ControllerBase
    {
        IWeeklyTourneyService _weeklyTourneyService;

        public WeeklyTourneyController(IWeeklyTourneyService weeklyTourneyService)
        {
            _weeklyTourneyService = weeklyTourneyService;
        }

        [HttpPost("GetCurrentWeeklyTourneyId")]
        public async Task<IActionResult> GetCurrentWeeklyTourneyId()
        {
            var result = await _weeklyTourneyService.GetCurrentTourneyId();

            return Ok(result);
        }

        [HttpPost("GetWeeklyTourneys")]
        public async Task<IActionResult> GetWeeklyTourneys()
        {
            var result = await _weeklyTourneyService.GetWeeklyTourneys();

            return Ok(result);
        }

        [HttpPost("GetWeeklyTournament")]
        public async Task<IActionResult> GetWeeklyTournament(WeeklyTourneyIdRequest request)
        {
            var result = await _weeklyTourneyService.GetWeeklyTournament(request);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("WeeklyTourneyRegister")]
        public async Task<IActionResult> WeeklyTourneyRegister()
        {
            var userId = UserHelper.GetUserId(User);

            await _weeklyTourneyService.WeeklyTourneyRegister(userId);

            return Ok();
        }

        [Authorize]
        [HttpPost("WeeklyTourneyInvite")]
        public async Task<IActionResult> WeeklyTourneyInvite(WeeklyTourneyInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _weeklyTourneyService.WeeklyTourneyInvite(userId, request.InvitedId);

            return Ok();
        }

        [Authorize]
        [HttpPost("WeeklyTourneyAcceptDeclineInvite")]
        public async Task<IActionResult> WeeklyTourneyAcceptDeclineInvite(WeeklyTourneyAcceptDeclineInvite request)
        {
            var userId = UserHelper.GetUserId(User);

            await _weeklyTourneyService.WeeklyTourneyAcceptDeclineInvite(userId, request);

            return Ok();
        }

        [HttpPost("RandomizeNextStage")]
        public async Task<IActionResult> RandomizeNextStage(int request)
        {
            await _weeklyTourneyService.RandomizeTourneyNextStage(request);

            return Ok();
        }
    }
}
