namespace hqm_ranked_backend.Models.ViewModels
{
    public class PlayerInviteViewModel
    {
        public Guid InviteId { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
    }
}
