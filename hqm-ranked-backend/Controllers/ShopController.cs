using hqm_ranked_backend.Common;
using hqm_ranked_models.InputModels;
using hqm_ranked_services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        IShopService _shopService;
        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpPost("GetShopItems")]
        public async Task<IActionResult> GetShopItems()
        {
            int? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = UserHelper.GetUserId(User);
            }

            var result = await _shopService.GetShopItems(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("PurchaseShopItem")]
        public async Task<IActionResult> PurchaseShopItem(PurchaseShopItemRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _shopService.PurchaseShopItem(request, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("SelectShopItem")]
        public async Task<IActionResult> SelectShopItem(SelectShopItemRequest request)
        {
            var userId = UserHelper.GetUserId(User);

            await _shopService.SelectShopItem(request, userId);
            return Ok();
        }

        [HttpPost("GetShopSelects")]
        public async Task<IActionResult> GetShopSelects()
        {
            var result = await _shopService.GetShopSelects();
            return Ok(result);
        }
    }
}
