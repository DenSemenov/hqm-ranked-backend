using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ReplayData : AuditableEntity<Guid>
    {
        public Game Game { get; set; }
        public byte[] Data { get; set; }
    }
}
