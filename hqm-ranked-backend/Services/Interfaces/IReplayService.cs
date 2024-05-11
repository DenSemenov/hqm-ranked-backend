using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IReplayService
    {
        Task PushReplay(Guid gameId, byte[] data, string token);
        void RemoveOldReplays();
        Task<string> GetReplayData(ReplayRequest request);
        Task ParseReplay(ReplayRequest request);
        Task<ReplayViewerViewModel> GetReplayViewer(ReplayViewerRequest request);
    }
}
