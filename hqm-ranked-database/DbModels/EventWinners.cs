using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class EventWinners : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public Events Event { get; set; }
    }
}
