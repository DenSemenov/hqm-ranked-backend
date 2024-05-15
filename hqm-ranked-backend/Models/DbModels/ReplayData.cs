using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ReplayData : AuditableEntity<Guid>
    {
        public Game Game { get; set; }
        public string Url { get; set; }
        public uint Min { get; set; }
        public uint Max { get; set; }
        public List<ReplayFragment> ReplayFragments { get; set; }
        public List<ReplayChat> ReplayChats { get; set; } = new List<ReplayChat>();
        public List<ReplayGoal> ReplayGoals { get; set; } = new List<ReplayGoal>();
    }
}
