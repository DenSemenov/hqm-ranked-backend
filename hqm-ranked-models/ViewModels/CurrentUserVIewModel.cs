namespace hqm_ranked_backend.Models.ViewModels
{
    public class CurrentUserVIewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BanLastDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAcceptedRules { get; set; }
        public bool IsDiscordConnected { get; set; } = false;
        public string DiscordLogin { get; set; } = string.Empty;
    }
}
