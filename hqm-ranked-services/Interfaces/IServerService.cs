using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_models.ViewModels;

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
        Task<HeartbeatViewModel> Heartbeat(HeartbeatRequest request);
        Task<ReportViewModel> Report(ReportRequest request);
        Task Reset(ResetRequest request);
        Task Resign(ResignRequest request);
        Task<List<string>> GetReasons();
        Task<InstanceType> GetServerType(string token);
    }
}
