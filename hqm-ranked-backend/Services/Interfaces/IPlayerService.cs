using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<LoginResult?> Login(LoginRequest request);
        Task<LoginResult?> Register(RegistrationRequest request);
        Task<CurrentUserVIewModel> GetCurrentUser(int userId);
        Task ChangePassword(PasswordChangeRequest request, int userId);
        Task<string> ChangeNickname(NicknameChangeRequest request, int userId);
        Task AddPushToken(PushTokenRequest request, int userId);
        Task RemovePushToken(PushTokenRequest request, int userId);
        Task<PlayerNotificationsViewModel> GetPlayerNotifications(int userId);
        Task SavePlayerNotifications(int userId, PlayerNotificationsViewModel request);
    }
}
