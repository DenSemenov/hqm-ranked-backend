using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }


        [HttpPost("GetServers")]
        public async Task<IActionResult> GetServers()
        {
            var result = await _adminService.GetServers();

            return Ok(result);
        }

        [HttpPost("AddServer")]
        public async Task<IActionResult> AddServer(AddServerRequest request)
        {
            await _adminService.AddServer(request);

            return Ok();
        }

        [HttpPost("RemoveServer")]
        public async Task<IActionResult> RemoveServer(RemoveServerRequest request)
        {
            await _adminService.RemoveServer(request);

            return Ok();
        }

        [HttpPost("GetPlayers")]
        public async Task<IActionResult> GetPlayers()
        {
            var result = await _adminService.GetPlayers();

            return Ok(result);
        }

        [HttpPost("BanPlayer")]
        public async Task<IActionResult> BanPlayer(BanUnbanRequest request)
        {
            await _adminService.BanPlayer(request);

            return Ok();
        }

        [HttpPost("GetAdmins")]
        public async Task<IActionResult> GetAdmins()
        {
            var result = await _adminService.GetAdmins();

            return Ok(result);
        }

        [HttpPost("AddRemoveAdmin")]
        public async Task<IActionResult> AddRemoveAdmin(AddRemoveAdminRequest request)
        {
            await _adminService.AddRemoveAdmin(request);

            return Ok();
        }
        [HttpPost("GetSettings")]
        public async Task<IActionResult> GetSettings()
        {
            var result = await _adminService.GetSettings();

            return Ok(result);
        }
        [HttpPost("SaveSettings")]
        public async Task<IActionResult> SaveSettings(Setting request)
        {
            await _adminService.SaveSettings(request);

            return Ok();
        }

        [HttpPost("GetUnApprovedUsers")]
        public async Task<IActionResult> GetUnApprovedUsers()
        {
            var result = await _adminService.GetUnApprovedUsers();

            return Ok(result);
        }
        [HttpPost("ApproveUser")]
        public async Task<IActionResult> ApproveUser(IApproveRequest request)
        {
            await _adminService.ApproveUser(request);

            return Ok();
        }
    }
}
