using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        [HttpPost("GetTeamsState")]
        public async Task<IActionResult> GetTeamsState()
        {
            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = UserHelper.GetUserId(User);
            }

            var result = await _teamsService.GetTeamsState(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("CreateTeam")]
        public async Task<IActionResult> CreateTeam(CreateTeamRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.CreateTeam(request.Name, userId);
            return Ok();
        }
    }
}
