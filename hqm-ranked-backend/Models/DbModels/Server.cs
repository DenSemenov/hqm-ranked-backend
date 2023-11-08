using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Server : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public int PlayerCount { get; set; }
    }
}
