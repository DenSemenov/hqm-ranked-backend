using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class ReplayGoal
    {
        [Key]
        public Guid Id { get; set; }
        public uint Packet { get; set; }
        public string GoalBy { get; set; }
        public int Period { get; set; }
        public int Time { get; set; }
        public Player Player { get; set; }
        public string Url { get; set; }
        public ReplayData ReplayData { get; set; }
    }
}
