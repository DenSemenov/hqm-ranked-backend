using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public void RemoveOldReplays()
        {
            var settings = _dbContext.Settings.FirstOrDefault();
            if (settings != null)
            {
                _dbContext.ReplayData.Where(x => x.CreatedOn.AddDays(settings.ReplayStoreDays) < DateTime.UtcNow).ExecuteDelete();
            }
        }

        public async Task<string> GetReplayData(ReplayRequest request)
        {
            var data = String.Empty;

            var replayData = await _dbContext.ReplayData.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (replayData != null)
            {
                data = Convert.ToBase64String(replayData.Data);
            }

            return data;
        }
    }
}
