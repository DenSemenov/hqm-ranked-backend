﻿using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        IServerService _serverService;

        public ServerController(IServerService serverService)
        {
            _serverService = serverService;
        }

        [HttpPost("GetActiveServers")]
        public async Task<IActionResult> GetActiveServers()
        {
            var result = await _serverService.GetActiveServers();

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(ServerLoginRequest request)
        {
            var result = await _serverService.ServerLogin(request);

            return Ok(result);
        }

        [HttpPost("StartGame")]
        public async Task<IActionResult> StartGame(StartGameRequest request)
        {
            var result = await _serverService.StartGame(request);

            return Ok(result);
        }

        [HttpPost("Pick")]
        public async Task<IActionResult> Pick(PickRequest request)
        {
            await _serverService.Pick(request);

            return Ok();
        }

        [HttpPost("AddGoal")]
        public async Task<IActionResult> AddGoal(AddGoalRequest request)
        {
            await _serverService.AddGoal(request);

            return Ok();
        }

        [HttpPost("SaveGame")]
        public async Task<IActionResult> SaveGame(SaveGameRequest request)
        {
            await _serverService.SaveGame(request);

            return Ok();
        }
    }
}
