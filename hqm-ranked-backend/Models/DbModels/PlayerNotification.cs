using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class PlayerNotification : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public string Token { get; set; } = String.Empty;
        public int LogsCount { get; set; } = 1;
        public NotifyType GameStarted { get; set; } = NotifyType.Enabled;
        public NotifyType GameEnded { get; set; } = NotifyType.Enabled;
    }

    public enum NotifyType
    {
        Disabled,
        Enabled,
        EnabledWithMe
    }
}
