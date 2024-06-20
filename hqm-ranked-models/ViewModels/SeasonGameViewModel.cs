using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class SeasonGameViewModel
    {
        public Guid GameId { get; set; }
        public DateTime Date { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public string Status { get; set; }
        public Guid? ReplayId { get; set; }
        public bool HasReplayFragments { get; set; }
        public InstanceType InstanceType { get; set; }
        public Guid? RedTeamId { get; set; }
        public Guid? BlueTeamId { get; set; }
        public string RedTeamName { get; set; }
        public string BlueTeamName { get; set; }
        public List<GamePlayerItem> Players { get; set; }
    }

    public class GamePlayerItem { 
        public int Id { get; set; }
        public string Name { get; set; }
        public int Team { get; set; }
    }
}
