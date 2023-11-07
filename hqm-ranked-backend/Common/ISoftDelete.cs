namespace hqm_ranked_backend.Common
{
    public interface ISoftDelete
    {
        DateTime? DeletedOn { get; set; }
    }
}
