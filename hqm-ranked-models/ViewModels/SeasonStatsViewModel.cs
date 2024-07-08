namespace hqm_ranked_backend.Models.ViewModels
{
    public class SeasonStatsViewModel
    {
        public int Place { get; set; }
        public int PlaceWeekAgo { get; set; }
        public int PlayerId { get; set; }  
        public string Nickname { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Mvp { get; set; }
        public int Rating { get; set; }
        public int RatingWeekAgo { get; set; }
        public int Change { get; set; }
    }
}
