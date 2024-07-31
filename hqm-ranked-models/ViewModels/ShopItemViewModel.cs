using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class ShopItemViewModel
    {
        public Guid Id { get; set; }
        public ShopItemGroup Group { get; set; }
        public ShopItemType Type { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public bool IsPurchased { get; set; } = false;
        public bool IsSelected { get; set; } = false;
    }
}
