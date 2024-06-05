using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Reports : AuditableEntity<Guid>
    {
        public Game Game { get; set; }
        public Player From { get; set; }  
        public Player To { get; set; }
        public Rule Reason { get; set; }
        public int Tick { get; set; }
    }
}
