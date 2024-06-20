namespace hqm_ranked_backend.Services.Interfaces
{
    public interface INotificationService
    {
        public Task SendDiscordNotification(string serverName, int count, int teamMax);
        public Task SendDiscordEndGameNotification(string serverName);
        public Task SendDiscordStartGameNotification(string serverName);
        public Task SendDiscordResignedNotification(string serverName);
        public Task SendDiscordCanceledNotification(string serverName);
        public Task SendPush(string title, string body, List<string> tokens);
    }
}
