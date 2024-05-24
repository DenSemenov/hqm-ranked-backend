using hqm_ranked_backend.Common;
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

    }
}
