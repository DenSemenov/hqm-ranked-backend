using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using hqm_ranked_services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_services
{
    public class ShopService: IShopService
    {
        private RankedDb _dbContext;
        private IContractService _contractService;
        public ShopService(RankedDb dbContext, IContractService contractService)
        {
            _dbContext = dbContext;
            _contractService = contractService;
        }

        public async Task<List<ShopItemViewModel>> GetShopItems(int? userId)
        {
            var result = await _dbContext.ShopItems.Include(x=>x.ShopPurchases).ThenInclude(x=>x.Player).Select(x => new ShopItemViewModel
            {
                Id = x.Id,
                Cost = x.Cost,
                Description = x.Description,
                Group = x.Group,
                Type = x.Type,
                IsPurchased = x.ShopPurchases.Any(x=>x.Player.Id == userId),
                IsSelected = x.ShopPurchases.Any(x => x.Player.Id == userId) ? x.ShopPurchases.FirstOrDefault(x => x.Player.Id == userId).IsSelected: false,
            }).OrderBy(x=>x.Group).ThenBy(x=>x.Cost).ToListAsync();

            return result;
        }

        public async Task PurchaseShopItem(PurchaseShopItemRequest request, int userId)
        {
            var itemToPurchase = await _dbContext.ShopItems.FirstOrDefaultAsync(x => x.Id == request.Id);
            var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
            if (itemToPurchase != null && player !=null)
            {
                var coins = await _contractService.GetCoins(userId);

                if (coins >= itemToPurchase.Cost)
                {
                    _dbContext.ShopPurchases.Add(new hqm_ranked_database.DbModels.ShopPurchases
                    {
                        Player = player,
                        ShopItem = itemToPurchase,
                    });

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task SelectShopItem(SelectShopItemRequest request, int userId)
        {
            var groupItems = await _dbContext.ShopPurchases.Include(x => x.ShopItem).Include(x=>x.Player).Where(x => x.ShopItem.Group == request.Group && x.Player.Id == userId).ToListAsync();
            foreach (var groupItem in groupItems)
            {
                groupItem.IsSelected = false;
            }

            var itemToSelect = await _dbContext.ShopPurchases.Include(x => x.ShopItem).Include(x => x.Player).FirstOrDefaultAsync(x => x.ShopItem.Id == request.Id && x.Player.Id == userId);
            if (itemToSelect != null)
            {
                itemToSelect.IsSelected = true;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ShopSelectViewModel>> GetShopSelects()
        {
            var result = await _dbContext.ShopPurchases.Include(x=>x.ShopItem).Include(x=>x.Player).Where(x=>x.IsSelected).Select(x=>new ShopSelectViewModel
            {
                 PlayerId = x.Player.Id,
                 ShopItemType = x.ShopItem.Type,
                 ShopItemGroup = x.ShopItem.Group,
            }).ToListAsync();

            return result;
        }
    }
}
