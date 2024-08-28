using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class StoryViewModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public List<StoryGoalViewModel> Goals { get; set; }
    }

    public class StoryGoalViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid ReplayId { get; set; }  
        public uint Packet {  get; set; }
        public string Url { get; set; }
        public List<StoryLikeViewModel> Likes { get; set; } = new List<StoryLikeViewModel> { };
        public Music? Music { get; set; }
        public InstanceType InstanceType { get; set; }

    }

    public class StoryLikeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
