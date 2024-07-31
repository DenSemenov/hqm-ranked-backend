using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class ShopSelectViewModel
    {
        public int PlayerId { get; set; }
        public ShopItemType ShopItemType { get; set; }
        public ShopItemGroup ShopItemGroup { get; set; }
    }
}
