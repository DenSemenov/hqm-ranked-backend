using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class EventType : AuditableEntity<Guid>
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public int MinX { get; set; }
        [Required]
        public int MaxX { get; set; }
        [Required]
        public int MinY { get; set; }
        [Required]
        public int MaxY { get; set; }

    }
}
