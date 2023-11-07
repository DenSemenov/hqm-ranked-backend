using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Common
{
    public interface IEntity
    {
        List<DomainEvent> DomainEvents { get; }
    }

    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }
}
