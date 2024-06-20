namespace hqm_ranked_backend.Models.DbModels
{
    public abstract class DomainEvent
    {
        public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
    }
}
