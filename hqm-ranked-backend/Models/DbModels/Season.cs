using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Season : AuditableEntity<Guid>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
        [Required]
        public DateTime DateEnd { get; set; }
        public ICollection<Game> Games { get; set; }
        public Division Division { get; set; }
        public Guid DivisionId { get; set; }

    }
}
