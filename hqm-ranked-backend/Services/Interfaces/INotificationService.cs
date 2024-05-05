﻿namespace hqm_ranked_backend.Services.Interfaces
{
    public interface INotificationService
    {
        public Task SendDiscordNotification(string serverName, int count, int teamMax);
        public Task SendDiscordEndGameNotification(string serverName);
        public Task SendDiscordStartGameNotification(string serverName);
    }
}