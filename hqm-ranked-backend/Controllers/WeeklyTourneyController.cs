using hqm_ranked_backend.Common;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
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

        [HttpPost("GetCurrentWeeklyTournament")]
        public async Task<IActionResult> GetCurrentWeeklyTournament()
        {
            var result = await _weeklyTourneyService.GetCurrentWeeklyTournament();

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
    }
}
