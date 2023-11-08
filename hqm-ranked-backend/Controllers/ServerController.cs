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

        [HttpPost("ServerUpdate")]
        public async Task<IActionResult> ServerUpdate(ServerUpdateRequest request)
        {
           await _serverService.ServerUpdate(request);

            return Ok();
        }
    }
}
