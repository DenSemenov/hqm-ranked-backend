using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _playerService.Login(request);

            if (result == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
            else
            {
                return Ok(result);
            }
        }

        [Authorize]
        [HttpPost("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _playerService.GetCurrentUser(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(PasswordChangeRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.ChangePassword(request, userId);
            return Ok();
        }
    }
}
