using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Game : AuditableEntity<Guid>
    {
        [Required]
        public int RedScore { get; set; }
        [Required]
        public int BlueScore { get; set; }
        [Required]
        public Season Season { get; set; }
        [Required]
        public States State { get; set; }
        public ICollection<GamePlayer> GamePlayers { get; set; }
        public ICollection<ReplayData> ReplayDatas { get; set; }
        public Player Mvp { get; set; }
        public int MvpId { get; set; }
        public InstanceType InstanceType { get; set; } = InstanceType.Ranked;
        public Team? RedTeam { get; set; }
        public Guid? RedTeamId { get; set; }
        public Team? BlueTeam { get; set; }
        public Guid? BlueTeamId { get; set; }
        public int? RedPoints { get; set; }
        public int? BluePoints { get; set; }
    }
}
