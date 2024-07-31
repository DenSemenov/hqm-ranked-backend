using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class ShopItem : AuditableEntity<Guid>
    {
        public ShopItemGroup Group { get; set; }
        public ShopItemType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Cost { get; set; } = 0;
        public ICollection<ShopPurchases> ShopPurchases { get; set; }
    }

    public enum ShopItemGroup
    {
        AvatarShape,
        Frame,
        Background
    }

    public enum ShopItemType
    {
        //shapes
        Circle,
        Square,
        Star,
        Hexagon,
        Heart,
        Rhomb,
        Pacman,

        //frames
        Lightning,
        Rain,
        Sphere,
        Triangles,
        CircleFrame,

        //backgrounds
        City,
        Solar,
        SunAndSea,
        DarkSun,
        Solar2,
        Tree

    }
}
