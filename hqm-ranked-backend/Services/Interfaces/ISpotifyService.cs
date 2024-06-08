namespace hqm_ranked_backend.Services.Interfaces
{
    public interface ISpotifyService
    {
        Task<string> GetSoundAsync();
    }
}
