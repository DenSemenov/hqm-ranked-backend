using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_database.DbModels
{
    public class TransferMarket : AuditableEntity<Guid>
    {
        public Team Team { get; set; }
        public List<Position> Positions { get; set; }
        public int Budget { get; set; }
        public ICollection<TransferMarketResponse> TransferMarketResponses { get; set; }
    }

    public enum Position
    {
        Gk,
        Def,
        Fwd
    }
}
