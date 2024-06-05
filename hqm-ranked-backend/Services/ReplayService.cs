using Hangfire;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReplayHandler.Classes;
using Serilog;

namespace hqm_ranked_backend.Services
{
    public class ReplayService: IReplayService
    {
        private RankedDb _dbContext;
        private IStorageService _storageService;
        private IReplayCalcService _replayCalcService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ReplayService(RankedDb dbContext, IWebHostEnvironment hostingEnvironment, IStorageService storageService, IReplayCalcService replayCalcService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
            _replayCalcService = replayCalcService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task PushReplay(Guid gameId, IFormFile file, string token)
        {
            try
            {
                var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == token);
                if (server != null)
                {
                    var game = await _dbContext.Games.FirstOrDefaultAsync(x => x.Id == gameId);
                    if (game != null)
                    {
                        var name = "replays/" + game.Id + ".hrp";

                        var type = await _storageService.UploadFile(name, file);

                        var entity = _dbContext.ReplayData.Add(new ReplayData
                        {
                            Game = game,
                            StorageType = type,
                            Url = name
                        });

                        await _dbContext.SaveChangesAsync();

                        BackgroundJob.Enqueue(() => _replayCalcService.ParseReplay(new ReplayRequest
                        {
                            Id = game.Id
                        }));
                    }
                    else
                    {
                        Log.Error(LogHelper.GetInfoLog("Game not found: " + gameId));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(LogHelper.GetErrorLog(ex.Message, ex.StackTrace));
            }
        }

        public void RemoveOldReplays()
        {
            var settings = _dbContext.Settings.FirstOrDefault();
            if (settings != null)
            {
                var dateToCheck = DateTime.UtcNow.AddDays(-settings.ReplayStoreDays);
                try
                {
                    var replaysToRemove = _dbContext.ReplayData.Include(x => x.ReplayFragments).Include(x => x.ReplayGoals).Where(x => x.CreatedOn < dateToCheck).ToList();
                    foreach (var replay in replaysToRemove)
                    {
                        foreach (var replayFragment in replay.ReplayFragments)
                        {
                            if (replayFragment.StorageType == Common.StorageType.Local)
                            {
                                var localPath = Path.Combine(_hostingEnvironment.ContentRootPath, "StaticFiles", settings.Id.ToString(), replayFragment.Data);
                                File.Delete(localPath);
                            }
                            else
                            {
                                _storageService.RemoveFile(replayFragment.Data).Wait();
                            }

                        }

                        foreach (var replayGoal in replay.ReplayGoals)
                        {
                            if (replayGoal.StorageType == Common.StorageType.Local)
                            {
                                var localPath = Path.Combine(_hostingEnvironment.ContentRootPath, "StaticFiles", settings.Id.ToString(), replayGoal.Url);
                                File.Delete(localPath);
                            }
                            else
                            {
                                _storageService.RemoveFile(replayGoal.Url).Wait();
                            }
                        }

                        if (replay.StorageType == Common.StorageType.Local)
                        {
                            var localPath = Path.Combine(_hostingEnvironment.ContentRootPath, "StaticFiles", settings.Id.ToString(), replay.Url);
                            File.Delete(localPath);
                        }
                        else
                        {
                            _storageService.RemoveFile(replay.Url).Wait();
                        }

                        _dbContext.ReplayData.Remove(replay);
                    }
                    _dbContext.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }
        public void ParseAllReplays()
        {
            var replayIds = _dbContext.ReplayData.Include(x => x.ReplayFragments).Include(x => x.Game).Where(x => x.ReplayFragments.Count == 0).Select(x => x.Game.Id).ToList();

            foreach (var replayId in replayIds)
            {
                _replayCalcService.ParseReplay(new ReplayRequest
                {
                    Id = replayId
                });
            }
        }

        public async Task<ReplayViewerViewModel> GetReplayViewer(ReplayViewerRequest request)
        {
            var result = new ReplayViewerViewModel();

            var query = await _dbContext.ReplayFragments.Include(x => x.ReplayData).ThenInclude(x => x.ReplayFragments).Where(x=>x.ReplayDataId == request.Id).Select(replayFragment => new
            {
                Data = replayFragment.Data,
                Index = replayFragment.Index,
                Fragments = replayFragment.ReplayData.ReplayFragments.OrderBy(x=>x.Index).Select(x => new ReplayViewerFragmentViewModel
                {
                    Index = x.Index,
                    Min = x.Min,
                    Max = x.Max,
                })
            }).FirstOrDefaultAsync(x =>  x.Index == request.Index); 

            if (query != null)
            {
                var path = query.Data;

                var json = await _storageService.LoadTextFile(path);
                var data = JsonConvert.DeserializeObject<ReplayTick[]>(json); 

                result.Index = query.Index;
                result.Data = data;
                result.Fragments = query.Fragments.ToList();
                result.GameId = _dbContext.ReplayData.FirstOrDefault(x => x.Id == request.Id).Game.Id;
            }

            return result;
        }

        public async Task<List<ReplayGoal>> GetReplayGoals(ReplayRequest request)
        {
            var result = new List<ReplayGoal>();

            var replayData = await _dbContext.ReplayData.Include(x => x.ReplayGoals).Where(x=>x.Id == request.Id).Select(x=>x.ReplayGoals).FirstOrDefaultAsync();
            if (replayData != null)
            {
                result = replayData.OrderBy(x=>x.Packet).ToList();
            }

            return result;
        }

        public async Task<List<ReplayChat>> GetReplayChatMessages(ReplayRequest request)
        {
            var result = new List<ReplayChat>();

            var replayData = await _dbContext.ReplayData.Include(x => x.ReplayChats).Where(x => x.Id == request.Id).Select(x => x.ReplayChats).FirstOrDefaultAsync();
            if (replayData != null)
            {
                result = replayData.OrderBy(x => x.Packet).ToList();
            }

            return result;
        }

        public async Task<List<ReplayHighlight>> GetReplayHighlights(ReplayRequest request)
        {
            var result = new List<ReplayHighlight>();

            var replayData = await _dbContext.ReplayData.Include(x => x.ReplayHighlight).Where(x => x.Id == request.Id).Select(x => x.ReplayHighlight).FirstOrDefaultAsync();
            if (replayData != null)
            {
                result = replayData.OrderBy(x => x.Packet).ToList();
            }

            return result;
        }

        public async Task<List<StoryViewModel>> GetReplayStories()
        {
            var result = new List<StoryViewModel>();

            var checkDate = DateTime.UtcNow.Date.AddDays(-1);
            var games = await _dbContext.ReplayData.Where(x => x.CreatedOn >= checkDate).Include(x => x.ReplayGoals).ThenInclude(x => x.Player).Include(x => x.ReplayGoals).ThenInclude(x => x.ReplayData).Include(x => x.ReplayGoals).ThenInclude(x => x.Likes).OrderByDescending(x => x.CreatedOn).ToListAsync();

            var players = games.SelectMany(x => x.ReplayGoals.Select(x => x.Player)).Distinct().ToList();
            var goals = games.SelectMany(x => x.ReplayGoals).ToList();

            foreach (var player in players)
            {
                var playerGoals = goals.Where(x => x.Player == player).Select(x => new StoryGoalViewModel
                {
                    Id = x.Id,
                    Date = x.ReplayData.CreatedOn,
                    Packet = x.Packet,
                    ReplayId = x.ReplayData.Id,
                    Likes = x.Likes.Select(l => new StoryLikeViewModel
                    {
                        Id = l.Id,
                        Name = l.Name,
                    }).ToList()
                }).DistinctBy(x => x.Packet).OrderBy(x => x.Date).ToList();

                result.Add(new StoryViewModel
                {
                    PlayerId = player.Id,
                    Name = player.Name,
                    Goals = playerGoals
                });
            }

            return result;
        }

        public async Task<ReplayViewerViewModel> GetStoryReplayViewer(StoryReplayViewerRequest request)
        {
            var result = new ReplayViewerViewModel();

            var goal = await _dbContext.ReplayGoals.FirstOrDefaultAsync(x=>x.Id == request.Id);

            if (goal != null)
            {
                var path = goal.Url;

                var json = await _storageService.LoadTextFile(path);
                var data = JsonConvert.DeserializeObject<ReplayTick[]>(json);

                result.Index = 0;
                result.Data = data;
                result.Fragments = new List<ReplayViewerFragmentViewModel> {
                   new ReplayViewerFragmentViewModel{
                    Index = 0,
                     Min = data.FirstOrDefault().PacketNumber,
                     Max = data.LastOrDefault().PacketNumber
                   }
                };
            }

            return result;
        }

        public async Task LikeStory(Guid storyId, int userId)
        {
            var goal = await _dbContext.ReplayGoals.FirstOrDefaultAsync(x => x.Id == storyId);

            if (goal != null)
            {
                var like = goal.Likes.FirstOrDefault(x => x.Id == userId);
                if (like !=null)
                {
                    goal.Likes.Remove(like);
                }
                else
                {
                    var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
                    goal.Likes.Add(player);
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
