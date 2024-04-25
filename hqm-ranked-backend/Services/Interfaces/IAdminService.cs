using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminServerViewModel>> GetServers();
        Task AddServer(AddServerRequest request);
        Task RemoveServer(RemoveServerRequest request);
    }
}
