using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class GamePlayer : AuditableEntity<Guid>
    {
        [Required]
        public Game Game { get; set; }
        [Required]
        public Player Player { get; set; }
        public int PlayerId { get; set; }
        [Required]
        public int Team { get; set; }
        [Required]
        public int Goals { get; set; }
        [Required]
        public int Assists { get; set; }
        [Required]
        public int Score { get; set; }
        public int Ping { get; set; }
        public string Ip { get; set; }
        public bool IsCaptain { get; set; }
        public int Shots { get; set; }
        public int Saves { get; set; }
        public int Conceded { get; set; }
        public double Possession { get; set; }
    }
}
