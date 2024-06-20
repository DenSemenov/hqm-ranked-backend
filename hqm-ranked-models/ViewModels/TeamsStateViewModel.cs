using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class TeamsStateViewModel
    {
        public bool CanCreateTeam { get; set; } = false;
        public bool IsCaptain { get; set; } = false;
        public bool IsAssistant { get; set; } = false;
        public int? CaptainId { get; set; }
        public int? AssistantId { get; set; }
        public int TeamsMaxPlayers { get; set; }
        public CurrentTeamViewModel? Team { get; set; }
    }

    public class CurrentTeamViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Budget { get; set; }
        public List<CurrentTeamBudgetViewModel> BudgetHistory { get; set; }
        public List<CurrentTeamPlayerViewModel> Players { get; set; }
    }

    public class CurrentTeamPlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
    }

    public class CurrentTeamBudgetViewModel
    {
        public DateTime Date { get; set; }
        public BudgetType Type { get; set; }
        public int Change { get; set; }
        public int? InvitedPlayerId { get; set; }
        public string? InvitedPlayerNickname { get; set; }

    }
}
