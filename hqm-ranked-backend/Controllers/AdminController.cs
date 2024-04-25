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
    }
}
