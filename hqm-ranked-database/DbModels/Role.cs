using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Role : AuditableEntity<Guid>
    {
        [Required]
        public string Name { get; set; }
    }
}
