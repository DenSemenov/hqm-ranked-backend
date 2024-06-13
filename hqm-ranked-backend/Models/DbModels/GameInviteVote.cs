using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class GameInviteVote : AuditableEntity<Guid>
    {
        public GameInvites GameInvite { get; set; }
        public Player Player { get; set; }
    }
}
