using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ReplayChat
    {
        [Key]
        public Guid Id { get; set; }
        public uint Packet { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public Player? Player { get; set; }
        public ReplayData ReplayData { get; set; }
    }
}
