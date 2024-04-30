﻿using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
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
        private IWebHostEnvironment _hostingEnvironment;

        public PlayerController(IPlayerService playerService, IWebHostEnvironment hostingEnvironment)
        {
            _playerService = playerService;
            _hostingEnvironment = hostingEnvironment;
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
            var result = await _playerService.GetCurrentUser(userId);
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
        [HttpPost("UploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            try
            {
                var userId = UserHelper.GetUserId(User);
                if (file != null)
                {
                    var path = "/avatars/" + userId + ".png";
                    using (var fileStream = new FileStream(_hostingEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
    }
}
