using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_database.DbModels
{
    public class ContractSelect : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public Contract Contract { get; set; }
        public bool Passed { get; set; } = false;
    }
}
