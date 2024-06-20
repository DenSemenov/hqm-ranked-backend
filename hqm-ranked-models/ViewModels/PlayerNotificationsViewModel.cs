using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class PlayerNotificationsViewModel
    {
        public string Token { get; set; } = "empty";
        public int LogsCount { get; set; } = 1;
        public NotifyType GameStarted { get; set; } = NotifyType.Enabled;
        public NotifyType GameEnded { get; set; } = NotifyType.Enabled;
    }
}
