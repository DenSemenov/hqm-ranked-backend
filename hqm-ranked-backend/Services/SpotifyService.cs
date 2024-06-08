using hqm_ranked_backend.Services.Interfaces;
using System.Text.Json;
using SpotifyAPI.Web;
using hqm_ranked_backend.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Serilog;
using hqm_ranked_backend.Helpers;

namespace hqm_ranked_backend.Services
{
    public class SpotifyService: ISpotifyService
    {
        private RankedDb _dbContext;
        public SpotifyService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetSoundAsync()
        {
            var result = String.Empty;

            var settings = await _dbContext.Settings.FirstOrDefaultAsync();
            if (!String.IsNullOrEmpty(settings.SpotifyPlaylist) && !String.IsNullOrEmpty(settings.SpotifyClientId) && !String.IsNullOrEmpty(settings.SpotifySecret))
            {

                var token = await GetToken(settings.SpotifyClientId, settings.SpotifySecret);
                var spotify = new SpotifyClient(token);

                var conf = new PlaylistGetItemsRequest
                {
                    Market = "ES",
                };
                var pl = await spotify.Playlists.GetItems(settings.SpotifyPlaylist, conf);

                var tracks = pl.Items.Where(x => ((dynamic)x.Track).PreviewUrl != null).Select(x => ((dynamic)x.Track)).ToList();

                var random = new Random();
                var randomTrack = tracks[random.Next(0, tracks.Count - 1)];
                var previewUrl = randomTrack.PreviewUrl;

                var artists = new List<string>();

                foreach (var artist in randomTrack.Artists)
                {
                    artists.Add(artist.Name);
                }

                Log.Information(LogHelper.GetInfoLog(previewUrl));
            }

            return result;
        }
        private async Task<string> GetToken(string clientId, string clientSecret)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("grant_type", "client_credentials"));
            collection.Add(new("client_id", clientId));
            collection.Add(new("client_secret", clientSecret));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var respJson = await response.Content.ReadAsStringAsync();
            var respObj = JsonSerializer.Deserialize<Root>(respJson);
            return respObj.access_token;
        }

        private class Root
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
        }
    }
}
