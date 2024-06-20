using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class TeamViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? CaptainId { get; set; }
        public int? AssistantId { get; set; }
        public int Games { get; set; }
        public int Goals { get; set; }
        public List<TeamPlayerViewModel> Players { get; set; } = new List<TeamPlayerViewModel>();
        public List<TeamBudgetViewModel> BudgetHistory { get; set; } = new List<TeamBudgetViewModel>();

    }

    public class TeamPlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TeamBudgetViewModel
    {
        public DateTime Date { get; set; }
        public BudgetType Type { get; set; }
        public int Change { get; set; }
        public int? InvitedPlayerId { get; set; }
        public string? InvitedPlayerNickname { get; set; }
    }
}
