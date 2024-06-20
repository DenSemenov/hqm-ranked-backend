namespace hqm_ranked_backend.Models.ViewModels
{
    public class SeasonTeamsStatsViewModel
    {
        public int Place { get; set; }
        public Guid TeamId { get; set; }
        public string Name { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Points { get; set; }
    }
}
