using hqm_ranked_backend.Common;
using System.ComponentModel.DataAnnotations;

namespace hqm_ranked_backend.Models.DbModels
{
    public class PlayerNotificationSetting : AuditableEntity<Guid>
    {
        public string Token { get; set; } = String.Empty;
        public int LogsCount { get; set; } = 1;
        public NotifyType GameStarted { get; set; } = NotifyType.On;
        public NotifyType GameEnded { get; set; } = NotifyType.On;
    }

    public enum NotifyType
    {
        None = 0,
        On = 1,
        OnWithMe = 2
    }
}
