using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class TeamPlayer : AuditableEntity<Guid>
    {
        public Team Team { get; set; }
        public Player Player { get; set; }
    }
}
