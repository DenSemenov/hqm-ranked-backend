namespace hqm_ranked_backend.Common
{
    public interface IAuditableEntity
    {
        public DateTime CreatedOn { get; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
