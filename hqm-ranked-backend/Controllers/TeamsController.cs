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

        [HttpPost("GetTeam")]
        public async Task<IActionResult> GetTeam(GetTeamRequest request)
        {
            var result = await _teamsService.GetTeam(request.TeamId);
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

        [Authorize]
        [HttpPost("LeaveTeam")]
        public async Task<IActionResult> LeaveTeam()
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.LeaveTeam(userId);
            return Ok();
        }

        [HttpPost("GetFreeAgents")]
        public async Task<IActionResult> GetFreeAgents()
        {
            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = UserHelper.GetUserId(User);
            }
            var result = await _teamsService.GetFreeAgents(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("InvitePlayer")]
        public async Task<IActionResult> InvitePlayer(InvitePlayerRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.InvitePlayer(userId, request.InvitedId);
            return Ok();
        }

        [Authorize]
        [HttpPost("CancelInvite")]
        public async Task<IActionResult> CancelInvite(CancelPlayerInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.CancelInvite(userId, request.InviteId);
            return Ok();
        }

        [Authorize]
        [HttpPost("GetInvites")]
        public async Task<IActionResult> GetInvites()
        {
            var userId = UserHelper.GetUserId(User);

            var result = await _teamsService.GetInvites(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("ApplyPlayerInvite")]
        public async Task<IActionResult> ApplyPlayerInvite(CancelPlayerInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.ApplyPlayerInvite(userId, request.InviteId);
            return Ok();
        }

        [Authorize]
        [HttpPost("DeclinePlayerInvite")]
        public async Task<IActionResult> DeclinePlayerInvite(CancelPlayerInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.DeclinePlayerInvite(userId, request.InviteId);
            return Ok();
        }

        [Authorize]
        [HttpPost("SellPlayer")]
        public async Task<IActionResult> SellPlayer(SellPlayerRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.SellPlayer(userId, request.PlayerId);
            return Ok();
        }

        [Authorize]
        [HttpPost("MakeCaptain")]
        public async Task<IActionResult> MakeCaptain(MakeCapOrAssistantRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.MakeCaptain(userId, request.PlayerId);
            return Ok();
        }

        [Authorize]
        [HttpPost("MakeAssistant")]
        public async Task<IActionResult> MakeAssistant(MakeCapOrAssistantRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.MakeAssistant(userId, request.PlayerId);
            return Ok();
        }

        [Authorize]
        [HttpPost("CreateGameInvite")]
        public async Task<IActionResult> CreateGameInvite(CreateGameInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            var result = await _teamsService.CreateGameInvite(userId, request.Date);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("RemoveGameInvite")]
        public async Task<IActionResult> RemoveGameInvite(RemoveGameInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.RemoveGameInvite(userId, request.InviteId);
            return Ok();
        }

        [Authorize]
        [HttpPost("GetGameInvites")]
        public async Task<IActionResult> GetGameInvites()
        {
            var userId = UserHelper.GetUserId(User);

            var result = await _teamsService.GetGameInvites(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("VoteGameInvite")]
        public async Task<IActionResult> VoteGameInvite(VoteGameInviteRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _teamsService.VoteGameInvite(userId, request.InviteId);
            return Ok();
        }

        [Authorize]
        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                var userId = UserHelper.GetUserId(User);

                await _teamsService.UploadAvatar(userId, file);

                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
    }
}
