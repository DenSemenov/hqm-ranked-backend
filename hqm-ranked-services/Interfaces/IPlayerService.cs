using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_models.DTO;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<LoginResult?> Login(LoginRequest request);
        Task<LoginResult?> Register(RegistrationRequest request);
        Task<CurrentUserVIewModel> GetCurrentUser(int userId, string ip, string userAgent, string acceptLang, string browser, string platform);
        Task ChangePassword(PasswordChangeRequest request, int userId);
        Task<string> ChangeNickname(NicknameChangeRequest request, int userId);
        Task AddPushToken(PushTokenRequest request, int userId);
        Task RemovePushToken(PushTokenRequest request, int userId);
        Task<PlayerNotificationsViewModel> GetPlayerNotifications(int userId);
        Task SavePlayerNotifications(int userId, PlayerNotificationsViewModel request);
        Task AcceptRules(int userId);
        Task<WebsiteSettingsViewModel> GetWebsiteSettings();
        Task SetDiscordByToken(int userId, string token);
        Task RemoveDiscord(int userId);
        Task<LoginResult?> LoginWithDiscord(DiscordAuthRequest request);
        Task<List<PlayerWarningViewModel>> GetPlayerWarnings(int userId);
        Task PutServerPlayerInfo(int playerId, string ip, hqm_ranked_database.DbModels.LoginInstance loginInstance, string userAgent, string acceptLang, string browser, string platform);
        Task CalcPlayersStats();
        Task<PlayerLoginInfo> GetIpInfo(string ip);
        Task SetShowLocation(SetShowLocationRequest request, int userId);
        Task<List<PlayerMapViewModel>> GetMap();
        Task ChangeLimitType(LimitTypeRequest request, int userId);
    }
}
