using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IReplayService
    {
        Task PushReplay(Guid gameId, IFormFile file, string token);
        void RemoveOldReplays();
        void ParseReplay(ReplayRequest request);
        Task<ReplayViewerViewModel> GetReplayViewer(ReplayViewerRequest request);
        Task<List<ReplayGoal>> GetReplayGoals(ReplayRequest request);
        Task<List<ReplayChat>> GetReplayChatMessages(ReplayRequest request);
    }
}
