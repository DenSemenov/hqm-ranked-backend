using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Budget : AuditableEntity<Guid>
    {
        public Team Team { get; set; }
        public BudgetType Type { get; set; }
        public int Change { get; set; }
        public Player? InvitedPlayer { get; set; }
    }

    public enum BudgetType
    {
        Start,
        Invite,
        Game,
        Leave,
        Sell
    }
}
