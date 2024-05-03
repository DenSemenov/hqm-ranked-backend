using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IServerService
    {
        Task<List<ActiveServerViewModel>> GetActiveServers();
        Task<ServerLoginViewModel> ServerLogin(ServerLoginRequest request);
        Task<StartGameViewModel> StartGame(StartGameRequest request);
        Task Pick(PickRequest request);
        Task AddGoal(AddGoalRequest request);
        Task<SaveGameViewModel> SaveGame(SaveGameRequest request);
        Task Heartbeat(HeartbeatRequest request);
    }
}
