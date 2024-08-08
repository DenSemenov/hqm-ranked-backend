using hqm_ranked_backend.Models.InputModels;
using ReplayHandler.Classes;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IReplayCalcService
    {
        Task ParseReplay(ReplayRequest request);
        Task<List<ReplayTick>> ProcessReplay(byte[] data);
    }
}
