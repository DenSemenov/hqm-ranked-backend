using hqm_ranked_backend.Models.InputModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IReplayCalcService
    {
        Task ParseReplay(ReplayRequest request);
    }
}
