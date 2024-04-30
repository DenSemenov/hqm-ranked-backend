using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminServerViewModel>> GetServers();
        Task AddServer(AddServerRequest request);
        Task RemoveServer(RemoveServerRequest request);
        Task <List<AdminPlayerViewModel>> GetPlayers();
        Task BanPlayer(BanUnbanRequest request);
        Task<List<AdminViewModel>> GetAdmins();
        Task AddRemoveAdmin(AddRemoveAdminRequest request);
        Task<Setting> GetSettings();
        Task SaveSettings(Setting request);
        Task<List<AdminPlayerViewModel>> GetUnApprovedUsers();
        Task ApproveUser(IApproveRequest request);
    }
}
