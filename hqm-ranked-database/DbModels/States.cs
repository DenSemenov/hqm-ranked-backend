using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class States : AuditableEntity<Guid>
    {
        [Required]
        public string Name { get; set; }
    }
}
