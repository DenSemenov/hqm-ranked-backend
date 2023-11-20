namespace hqm_ranked_backend.Models.ViewModels
{
    public class SeasonGameViewModel
    {
        public Guid GameId { get; set; }
        public DateTime Date { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public string Status { get; set; }
        public Guid TeamRedId { get; set; }
        public Guid TeamBlueId { get; set; }
        public string TeamNameRed { get; set; }
        public string TeamNameBlue { get; set; }
    }
}
