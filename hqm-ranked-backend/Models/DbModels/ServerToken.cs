using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ServerToken : AuditableEntity<Guid>
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public Division Division { get; set; }

    }
}
