using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Team : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public Player? Captain { get; set; }
        public Player? Assistant { get; set; }
        public Season Season { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<TeamPlayer> TeamPlayers { get; set; }

    }
}
