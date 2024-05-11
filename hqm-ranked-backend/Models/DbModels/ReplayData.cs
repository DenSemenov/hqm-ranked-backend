using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ReplayData : AuditableEntity<Guid>
    {
        public Game Game { get; set; }
        public byte[] Data { get; set; }
        public uint Min { get; set; }
        public uint Max { get; set; }
        public List<ReplayFragment> ReplayFragments { get; set; }
    }
}
