using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;

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
        public List<ReplayChat> ChatMessages { get; set; }
        public List<ReplayGoal> Goals { get; set; }
        public Guid? ReplayId { get; set; }
        public bool HasReplayFragments { get; set; }
        public string? ReplayUrl { get; set; }
        public InstanceType InstanceType { get; set; }
        public Guid? RedTeamId { get; set; }
        public Guid? BlueTeamId { get; set; }
        public string RedTeamName { get; set; }
        public string BlueTeamName { get; set; }
        public int RedPoints { get; set; }
        public int BluePoints { get; set; }
    }

    public class GameDataPlayerViewModel
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Score { get; set; }  
        public int Team { get; set; }
        public int Shots { get; set; }
        public int Saves { get; set; }
        public int Conceded { get; set; }
        public double Possession { get; set; }
    }
}
