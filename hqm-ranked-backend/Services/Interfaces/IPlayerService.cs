using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<LoginResult?> Login(LoginRequest request);
        Task<LoginResult?> Register(RegistrationRequest request);
        Task<CurrentUserVIewModel> GetCurrentUser(Guid userId);
        Task ChangePassword(PasswordChangeRequest request, Guid userId);
    }
}
