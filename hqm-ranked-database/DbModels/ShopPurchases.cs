using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class ShopPurchases : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public ShopItem ShopItem { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
