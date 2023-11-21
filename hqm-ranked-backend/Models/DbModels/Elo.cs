using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Elo : AuditableEntity<Guid>
    {
        [Required]
        public Season Season { get; set; }
        [Required]
        public Player Player { get; set; }
        public int Value { get; set; }
    }
}
