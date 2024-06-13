namespace hqm_ranked_backend.Models.ViewModels
{
    public class TeamsStateViewModel
    {
        public bool CanCreateTeam { get; set; } = false;
        public bool IsCaptain { get; set; } = false;
        public bool IsAssistant { get; set; } = false;
        public CurrentTeamViewModel? Team { get; set; }
    }

    public class CurrentTeamViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
