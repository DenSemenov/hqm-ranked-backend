using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface ITeamsService
    {
        Task<TeamsStateViewModel> GetTeamsState(int? userId);
        Task CreateTeam(string name, int userId);
    }
}
