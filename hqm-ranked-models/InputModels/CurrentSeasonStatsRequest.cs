namespace hqm_ranked_backend.Models.InputModels
{
    public class CurrentSeasonStatsRequest
    {
        public Guid SeasonId { get; set; }
        public int Offset { get; set;}
        public DateTime? DateAgo { get; set; } = null;
    }
}
