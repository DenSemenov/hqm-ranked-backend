using hqm_ranked_backend.Common;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("GetCurrentEvent")]
        public async Task<IActionResult> GetCurrentEvent()
        {
            var result = await _eventService.GetCurrentEvent();
            return Ok(result);
        }
    }
}
