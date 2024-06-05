using hqm_ranked_backend.Models.InputModels;
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
            Thread.Sleep(3000);

            var result = await _serverService.SaveGame(request);

            return Ok(result);
        }

        [HttpPost("Heartbeat")]
        public async Task<IActionResult> Heartbeat(HeartbeatRequest request)
        {
            await _serverService.Heartbeat(request);

            return Ok();
        }

        [HttpPost("Report")]
        public async Task<IActionResult> Report(ReportRequest request)
        {
            var result = await _serverService.Report(request);

            return Ok(result);
        }

        [HttpPost("Resign")]
        public async Task<IActionResult> Resign(ResignRequest request)
        {
            await _serverService.Resign(request);

            return Ok();
        }

        [HttpPost("Reset")]
        public async Task<IActionResult> Reset(ResetRequest request)
        {
            await _serverService.Reset(request);

            return Ok();
        }

        [HttpPost("GetReasons")]
        public async Task<IActionResult> GetReasons()
        {
            var result = await _serverService.GetReasons();

            return Ok(result);
        }
    }
}
