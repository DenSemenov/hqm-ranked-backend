using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<Music?> GetSoundAsync();
        Task GetPlaylist();
    }
}
