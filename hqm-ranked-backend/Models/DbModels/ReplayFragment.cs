using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ReplayFragment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ReplayDataId { get; set; }
        public ReplayData ReplayData { get; set; }
        public int Index { get; set; }
        public string Data { get; set; }
        public StorageType StorageType { get; set; } = StorageType.S3;
        public uint Min { get; set; }
        public uint Max { get; set; }
    }
}
