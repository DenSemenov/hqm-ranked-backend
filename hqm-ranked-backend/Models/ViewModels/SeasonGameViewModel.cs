namespace hqm_ranked_backend.Models.ViewModels
{
    public class SeasonGameViewModel
    {
        public Guid GameId { get; set; }
        public DateTime Date { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public string Status { get; set; }
        public List<GamePlayerItem> Players { get; set; }
    }

    public class GamePlayerItem { 
        public int Id { get; set; }
        public string Name { get; set; }
        public int Team { get; set; }
    }
}
