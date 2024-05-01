using hqm_ranked_backend.Models.InputModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IReplayService
    {
        Task PushReplay(Guid gameId, byte[] data, string token);
        Task RemoveOldReplays();
        Task<HttpResponseMessage> GetReplayData(ReplayRequest request);
    }
}
