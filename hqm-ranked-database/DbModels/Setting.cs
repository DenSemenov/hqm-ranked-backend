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
        public int ShadowBanReportsCount { get; set; } = 5;
        public int StartingElo { get; set; } = 1000;
        public string S3Domain { get; set; } = String.Empty;
        public string S3Bucket { get; set; } = String.Empty;
        public string S3User { get; set; } = String.Empty;
        public string S3Key { get; set; } = String.Empty;
        public string PushJson { get; set; } = String.Empty;
        public string SpotifyClientId { get; set; } = String.Empty;
        public string SpotifySecret { get; set; } = String.Empty;
        public string SpotifyPlaylist { get; set; } = String.Empty;
        public int TeamsMaxPlayer { get; set; } = 4;
        public string DiscordAppClientId { get; set; }
        public string DiscordNewsWebhook { get; set; }
        public string DiscordJoinLink { get; set; }
        public bool DiscordApprove { get; set; } = false;
        public int RequiredGamesCount { get; set; } = 0;
        public string WebUrl { get; set; } = String.Empty;
        public string TelegramBotToken { get; set; } = String.Empty;
        public long TelegramGroupId { get; set; } = 0;
        public int NotificationThreadId { get; set; } = 0;
        public int NewsThreadId { get; set; } = 0;
        public string TelegramJoinLink { get; set; }

    }
}
