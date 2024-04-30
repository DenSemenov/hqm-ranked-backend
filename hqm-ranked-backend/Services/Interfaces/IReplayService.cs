using hqm_ranked_backend.Models.InputModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IReplayService
    {
        Task PushReplay(byte[] data, string token);
    }
}
