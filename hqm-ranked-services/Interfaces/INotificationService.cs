using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_database.DbModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface INotificationService
    {
        public Task SendDiscordNotification(string serverName, int count, int teamMax);
        public Task SendDiscordEndGameNotification(string serverName);
        public Task SendDiscordStartGameNotification(string serverName, List<string> ids);
        public Task SendDiscordResignedNotification(string serverName);
        public Task SendDiscordCanceledNotification(string serverName);
        public Task SendPush(string title, string body, List<string> tokens);
        public Task SendDiscordNewsAward(Award award, string playerName);
        public Task SendDiscordTeamInvite(GameInvites gameInvite);
        public Task SendDiscordNicknameChange(Player player, string newNickname);
        public Task SendDiscordTeamsGame(GameInvites gameInvite, string team1, string team2);
        public Task SendDiscordRegistrationStarted(string name, string url);
        public Task SendDiscordTourneyStarted(string name, string url);
    }
}
