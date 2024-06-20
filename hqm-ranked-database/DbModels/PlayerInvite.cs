using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class PlayerInvite : AuditableEntity<Guid>
    {
        public Team Team { get; set; }
        public Player Player { get; set; }
        public Budget Budget { get; set; }
    }
}
