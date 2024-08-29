using hqm_ranked_backend.Common;
using hqm_ranked_database.DbModels;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Player : AuditableEntity<int>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public Role Role { get; set; }
        public ICollection<GamePlayer> GamePlayers { get; set; }
        public ICollection<Bans> Bans { get; set; }
        public ICollection<NicknameChanges> NicknameChanges { get; set; }
        [Required]
        public bool IsApproved { get; set; } = true;
        [Required]
        public bool IsAcceptedRules { get; set; } = false;
        public PlayerCost? Cost { get; set; }
        public string DiscordId { get; set; } = string.Empty;
        public string DiscordNickname { get; set; } = string.Empty;
        public ICollection<Award> Awards { get; set; }
        public ICollection<PlayerLogin> PlayerLogins { get; set; }
        public PlayerCalcStats? PlayerCalcStats { get; set; }
        public Guid? PlayerCalcStatsId { get; set; }
        public bool ShowLocation { get; set; } = false;
        public ICollection<ContractSelect> ContractSelects { get; set; }
        public LimitsType LimitType { get; set; } = LimitsType.New;
        public double LimitTypeValue { get; set; } = 0.01;
        public ICollection<Reports> Reports { get; set; }
    }

    public enum LimitsType
    {
        Default,
        New,
        None
    }
}
