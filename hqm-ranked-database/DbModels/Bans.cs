using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Bans : AuditableEntity<Guid>
    {
        [Required]
        public Player BannedPlayer { get; set; }
        [Required]
        public int Days { get; set; }
    }
}
