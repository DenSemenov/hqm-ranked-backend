using hqm_ranked_backend.Common;
using Microsoft.AspNetCore.Http;
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
        [Required]
        public Player Mvp { get; set; }
    }
}
