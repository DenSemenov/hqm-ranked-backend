namespace hqm_ranked_backend.Models.ViewModels
{
    public class GameDataViewModel
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public DateTime Date { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public List<GameDataPlayerViewModel> Players { get; set; }
        public Guid? ReplayId { get; set; }
        public bool HasReplayFragments { get; set; }
    }

    public class GameDataPlayerViewModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Score { get; set; }  
        public int Team { get; set; }
    }
}
