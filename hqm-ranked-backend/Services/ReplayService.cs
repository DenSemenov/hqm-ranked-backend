using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;

namespace hqm_ranked_backend.Services
{
    public class ReplayService:IReplayService
    {
        private RankedDb _dbContext;
        public ReplayService(RankedDb dbContext, IWebHostEnvironment hostingEnvironment, IImageGeneratorService imageGeneratorService)
        {
            _dbContext = dbContext;
        }

        public async Task PushReplay(byte[] data, string token)
        {

        }
    }
}
