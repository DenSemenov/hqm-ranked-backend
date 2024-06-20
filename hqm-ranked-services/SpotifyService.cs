using hqm_ranked_backend.Services.Interfaces;
using System.Text.Json;
using hqm_ranked_backend.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;

namespace hqm_ranked_backend.Services
{
    public class SpotifyService: ISpotifyService
    {
        private RankedDb _dbContext;

        public SpotifyService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Music?> GetSoundAsync()
        {
            Music? result = null;

            if (await _dbContext.Music.AnyAsync())
            {
                result = _dbContext.Music.ToList().OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            }

            return result;
        }

        public async Task GetPlaylist()
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();
            if (!String.IsNullOrEmpty(settings.SpotifyPlaylist) && !String.IsNullOrEmpty(settings.SpotifyClientId) && !String.IsNullOrEmpty(settings.SpotifySecret))
            {
                try
                {
                    var token = await GetToken(settings.SpotifyClientId, settings.SpotifySecret);
                    var spotify = new SpotifyClient(token);

                    var tracks = new List<dynamic>();

                    var offset = 0;
                    var total = 0;
                   
                    while (total > offset || total == 0)
                    {
                        var conf = new PlaylistGetItemsRequest
                        {
                            Market = "TR",
                            Limit = 100,
                            Offset = offset,
                        };
                        var pl = await spotify.Playlists.GetItems(settings.SpotifyPlaylist, conf);
                        tracks.AddRange(pl.Items.Where(x => ((dynamic)x.Track).PreviewUrl != null).Select(x => ((dynamic)x.Track)).ToList());

                        total = (int)pl.Total;
                        offset += 100;
                    }

                    foreach (var track in tracks)
                    {
                        var artists = new List<string>();
                        foreach (var artist in track.Artists)
                        {
                            artists.Add(artist.Name);
                        }

                        var name = track.Name as string;
                        var artistsName = String.Join(", ", artists) as string;
                        var imagesCount = track.Album.Images.Count;
                        var imageUrl = imagesCount != 0 ? track.Album.Images[imagesCount - 1].Url : String.Empty;
                        var previewUrl = track.PreviewUrl;

                        if (!_dbContext.Music.Any(x => x.Name == name && x.Title == artistsName))
                        {
                            _dbContext.Music.Add(new Music
                            {
                                Name = name,
                                Title = artistsName,
                                ImageUrl = imageUrl,
                                Url = previewUrl,
                            });
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                }
                catch { }
            }
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
