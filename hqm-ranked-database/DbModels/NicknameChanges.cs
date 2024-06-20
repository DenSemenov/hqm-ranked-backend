using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class NicknameChanges : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public string OldNickname { get; set; }
    }
}
