using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_services.Interfaces
{
    public interface IShopService
    {
        Task<List<ShopItemViewModel>> GetShopItems(int? userId);
        Task PurchaseShopItem(PurchaseShopItemRequest request, int userId);
        Task SelectShopItem(SelectShopItemRequest request, int userId);
        Task<List<ShopSelectViewModel>> GetShopSelects();
    }
}
