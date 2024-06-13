using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class PlayerCost : AuditableEntity<Guid>
    {
        public int Cost { get; set; }
    }
}
