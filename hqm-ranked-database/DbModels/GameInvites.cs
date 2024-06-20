using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class GameInvites : AuditableEntity<Guid>
    {
        public Team InvitedTeam { get; set; }
        public DateTime Date {  get; set; }
        public Game? Game { get; set; }
        public ICollection<GameInviteVote> GameInviteVotes { get; set; }
    }
}
