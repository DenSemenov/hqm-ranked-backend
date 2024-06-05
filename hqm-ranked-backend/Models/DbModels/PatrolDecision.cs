using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class PatrolDecision : AuditableEntity<Guid>
    {
        public Player From { get; set; }
        public Reports Report { get; set; }
        public bool IsReported { get; set;}
    }
}
