using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Setting : AuditableEntity<Guid>
    {
        public int NicknameChangeDaysLimit { get; set; } = 30;
        public bool NewPlayerApproveRequired { get; set; } = false;
        public string Rules { get; set; } = String.Empty;
        public int ReplayStoreDays { get; set; } = 10;
        public int NextGameCheckGames { get; set; } = 5;
        public string DiscordNotificationWebhook { get; set; } = String.Empty;
        public int WebhookCount { get; set; } = 0;
    }
}
