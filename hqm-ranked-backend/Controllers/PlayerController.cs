using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_models.InputModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        IPlayerService _playerService;
        private IStorageService _storageService;

        public PlayerController(IPlayerService playerService, IStorageService storageService)
        {
            _playerService = playerService;
            _storageService = storageService;
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

        [HttpPost("LoginWithDiscord")]
        public async Task<IActionResult> LoginWithDiscord(DiscordAuthRequest request)
        {
            var result = await _playerService.LoginWithDiscord(request);

            if (result == null)
            {
                return BadRequest(new { errorText = "User with this discord not found" });
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            var result = await _playerService.Register(request);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = UserHelper.GetUserId(User);
            var ip = Request.Headers["X-Real-IP"].FirstOrDefault();
            var result = await _playerService.GetCurrentUser(userId, ip);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("ChangeNickname")]
        public async Task<IActionResult> ChangeNickname(NicknameChangeRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _playerService.ChangeNickname(request, userId);
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

        [Authorize]
        [HttpPost("AddPushToken")]
        public async Task<IActionResult> AddPushToken(PushTokenRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.AddPushToken(request, userId);
            return Ok();
        }


        [Authorize]
        [HttpPost("RemovePushToken")]
        public async Task<IActionResult> RemovePushToken(PushTokenRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.RemovePushToken(request, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("GetPlayerNotifications")]
        public async Task<IActionResult> GetPlayerNotifications()
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _playerService.GetPlayerNotifications(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("SavePlayerNotifications")]
        public async Task<IActionResult> SavePlayerNotifications(PlayerNotificationsViewModel request)
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.SavePlayerNotifications(userId, request);
            return Ok();
        }

        [Authorize]
        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                var userId = UserHelper.GetUserId(User);
                if (file != null)
                {
                    var name = "images/" + userId + ".png";
                    await _storageService.UploadFile(name, file);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [Authorize]
        [HttpPost("AcceptRules")]
        public async Task<IActionResult> AcceptRules()
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.AcceptRules(userId);
            return Ok();
        }

        [HttpPost("GetWebsiteSettings")]
        public async Task<IActionResult> GetWebsiteSettings()
        {
            var result = await _playerService.GetWebsiteSettings();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("SetDiscordByToken")]
        public async Task<IActionResult> SetDiscordByToken(DiscordAuthRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.SetDiscordByToken(userId, request.Token);
            return Ok();
        }

        [Authorize]
        [HttpPost("RemoveDiscord")]
        public async Task<IActionResult> RemoveDiscord()
        {
            var userId = UserHelper.GetUserId(User);
            await _playerService.RemoveDiscord(userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("GetPlayerWarnings")]
        public async Task<IActionResult> GetPlayerWarnings()
        {
            var userId = UserHelper.GetUserId(User);
            var result = await _playerService.GetPlayerWarnings(userId);
            return Ok(result);
        }

        [HttpPost("GetIpInfo")]
        public async Task<IActionResult> GetIpInfo()
        {
            var ip = Request.Headers["X-Real-IP"].FirstOrDefault();
            var result = await _playerService.GetIpInfo(ip);
            return Ok(new
            {
                Headers = Request.Headers,
                Result = result
            });
        }
    }
}
