using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Events : AuditableEntity<Guid>
    {
        public DateTime Date { get; set; }
        public EventType EventType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
