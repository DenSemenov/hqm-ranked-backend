using Hangfire;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReplayHandler.Classes;

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
                    var entity = _dbContext.ReplayData.Add(new ReplayData
                    {
                         Game = game,
                         Data = data,
                    });

                    await _dbContext.SaveChangesAsync();

                    BackgroundJob.Enqueue(() => ParseReplay(new ReplayRequest
                    {
                        Id = entity.Entity.Id
                    }));
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

        public void ParseReplay(ReplayRequest request)
        {
            var replayData = _dbContext.ReplayData.Include(x=>x.Game).FirstOrDefault(x=>x.Game.Id == request.Id);
            if (replayData != null)
            {
                var result = ReplayHandler.ReplayHandler.ParseReplay(replayData.Data);

                var fragmentLenght = 1000;

                var count = Math.Ceiling((double)result.Count / (double)fragmentLenght);

                replayData.Min = result.FirstOrDefault().PacketNumber;
                replayData.Max = result.LastOrDefault().PacketNumber;

                replayData.ReplayFragments = new List<ReplayFragment>();

                for (int i = 0; i < count; i++)
                {
                    var fragment = result.Skip(i* fragmentLenght).Take(fragmentLenght).ToArray();

                    replayData.ReplayFragments.Add(new ReplayFragment
                    {
                         Data = JsonConvert.SerializeObject(fragment),
                         Index = i,
                         Min = fragment.Min(x=>x.PacketNumber),
                         Max = fragment.Max(x=>x.PacketNumber)
                    });
                }

                _dbContext.SaveChanges();
            }
        }

        public async Task<ReplayViewerViewModel> GetReplayViewer(ReplayViewerRequest request)
        {
            var result = new ReplayViewerViewModel();

            var query = await _dbContext.ReplayFragments.Include(x => x.ReplayData).ThenInclude(x => x.ReplayFragments).Select(replayFragment => new
            {
                Data = replayFragment.Data,
                Index = replayFragment.Index,
                Fragments = replayFragment.ReplayData.ReplayFragments.OrderBy(x=>x.Index).Select(x => new ReplayViewerFragmentViewModel
                {
                    Index = x.Index,
                    Min = x.Min,
                    Max = x.Max,
                })
            }).FirstOrDefaultAsync(x => x.Index == request.Index); ;

            if (query != null)
            {
                var data = JsonConvert.DeserializeObject<ReplayTick[]>(query.Data); 

                result.Index = query.Index;
                result.Data = data;
                result.Fragments = query.Fragments.ToList();
            }

            return result;
        }
    }
}
