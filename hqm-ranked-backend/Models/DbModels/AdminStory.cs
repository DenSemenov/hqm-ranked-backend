using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class AdminStory : AuditableEntity<Guid>
    {
        public string Text { get; set; }
        public bool Expiration { get; set; }
    }
}
