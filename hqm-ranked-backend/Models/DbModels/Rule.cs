using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Rule : AuditableEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
