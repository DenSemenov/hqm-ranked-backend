using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;

namespace hqm_ranked_backend.Services
{
    public class ReplayService:IReplayService
    {
        private RankedDb _dbContext;
        public ReplayService(RankedDb dbContext, IWebHostEnvironment hostingEnvironment, IImageGeneratorService imageGeneratorService)
        {
            _dbContext = dbContext;
        }

        public async Task PushReplay(Guid gameId, byte[] data, string token)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == token);
            if (server != null)
            {
                var game = await _dbContext.Games.FirstOrDefaultAsync(x=>x.Id == gameId);
                if (game != null)
                {
                    _dbContext.ReplayData.Add(new ReplayData
                    {
                         Game = game,
                         Data = data,
                    });

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveOldReplays()
        {
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();
            if (settings != null)
            {
                await _dbContext.ReplayData.Where(x => x.CreatedOn.AddDays(settings.ReplayStoreDays) < DateTime.UtcNow).ExecuteDeleteAsync();
            }
        }

        public async Task<HttpResponseMessage> GetReplayData(ReplayRequest request)
        {
            byte[] data = [];

            var replayData = await _dbContext.ReplayData.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (replayData != null)
            {
                data = replayData.Data;
            }
            
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(data)
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = request.Id.ToString() + ".hrp"
                };
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}
