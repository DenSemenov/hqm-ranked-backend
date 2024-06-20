using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Avatar: AuditableEntity<Guid>
    {
        [Required]
        public Player Player { get; set; }
        public byte[] Image { get; set; }
        public byte[] Thumbnail { get; set; }
    }
}
