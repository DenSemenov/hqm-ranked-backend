namespace hqm_ranked_backend.Models.ViewModels
{
    public class FreeAgentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public Guid? InviteId { get; set; }
    }
}
