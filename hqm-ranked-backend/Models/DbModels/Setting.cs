using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Setting : AuditableEntity<Guid>
    {
        public int NicknameChangeDaysLimit { get; set; } = 30;
        public bool NewPlayerApproveRequired { get; set; } = false;
        public string Rules { get; set; } = String.Empty;

    }
}
