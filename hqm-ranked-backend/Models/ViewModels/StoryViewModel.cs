namespace hqm_ranked_backend.Models.ViewModels
{
    public class StoryViewModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public List<Guid> GoalIds { get; set; }
    }
}
