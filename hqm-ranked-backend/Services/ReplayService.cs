using Hangfire;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReplayHandler.Classes;
using System.Linq;

namespace hqm_ranked_backend.Services
{
    public class ReplayService: IReplayService
    {
        private RankedDb _dbContext;
        private IStorageService _storageService;
        public ReplayService(RankedDb dbContext, IWebHostEnvironment hostingEnvironment, IStorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
        }

        public async Task PushReplay(Guid gameId, IFormFile file, string token)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == token);
            if (server != null)
            {
                var game = await _dbContext.Games.FirstOrDefaultAsync(x=>x.Id == gameId);
                if (game != null)
                {
                    var name = "replays/" + game.Id + ".hrp";

                    if (await _storageService.UploadFile(name, file))
                    {

                        var entity = _dbContext.ReplayData.Add(new ReplayData
                        {
                            Game = game,
                            Url = name
                        });

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public void RemoveOldReplays()
        {
            var settings = _dbContext.Settings.FirstOrDefault();
            if (settings != null)
            {
                var replaysToRemove = _dbContext.ReplayData.Include(x => x.ReplayFragments).Where(x => x.CreatedOn.AddDays(settings.ReplayStoreDays) < DateTime.UtcNow).ToList();
                foreach (var replay in replaysToRemove)
                {
                    foreach (var replayFragment in replay.ReplayFragments)
                    {
                        File.Delete(replayFragment.Data);
                    }

                    _dbContext.ReplayData.Remove(replay);
                }
                _dbContext.SaveChanges();
            }
        }
        [DisableConcurrentExecution(10)]
        public void ParseAllReplays()
        {
            var isPlayersOnServer = _dbContext.Servers.Any(x => x.LoggedIn > 4 || x.State != 0);
            if (!isPlayersOnServer)
            {
                var replayIds = _dbContext.ReplayData.Include(x => x.ReplayFragments).Include(x=>x.Game).Where(x => x.ReplayFragments.Count == 0).Select(x => x.Game.Id).ToList();

                foreach(var replayId in replayIds)
                {
                    ParseReplay(new ReplayRequest
                    {
                        Id = replayId
                    });

                    if (_dbContext.Servers.Any(x => x.LoggedIn > 4 || x.State != 0))
                    {
                        break;
                    }
                }
            }
        }

        public void ParseReplay(ReplayRequest request)
        {
            var replayData = _dbContext.ReplayData.Include(x => x.Game).FirstOrDefault(x => x.Game.Id == request.Id);
            if (replayData != null)
            {
                var storageUrl = String.Empty;

                var setting = _dbContext.Settings.FirstOrDefault();
                if (setting != null)
                {
                    storageUrl = String.Format("https://{0}/{1}/{2}/", setting.S3Domain, setting.S3Bucket, setting.Id);
                }

                var client = new System.Net.WebClient();
                var data = client.DownloadData(storageUrl + replayData.Url);

                var result = ReplayHandler.ReplayHandler.ParseReplay(data);

                var processedData = ReplayDataHelper.GetReplayCalcData(result);

                var chatsToAdd = new List<ReplayChat>();

                foreach (var msg in processedData.Chats)
                {
                    chatsToAdd.Add(new ReplayChat
                    {
                        Packet = msg.Packet,
                        Text = msg.Text,
                        ReplayData = replayData
                    });
                }

                var goalsToAdd = new List<ReplayGoal>();

                foreach (var goal in processedData.Goals)
                {
                    var id = Guid.NewGuid();

                    var goalTicks = result.Skip((int)(goal.Packet - 250)).Take(300).ToList();

                    var json = JsonConvert.SerializeObject(goalTicks);
                    var path = "replayGoals/" + request.Id.ToString() + id.ToString() + ".json";

                    _storageService.UploadTextFile(path, json).Wait();

                    var player = _dbContext.Players.FirstOrDefault(x => x.Name == goal.GoalBy);

                    goalsToAdd.Add(new ReplayGoal
                    {
                        Id = id,
                        Packet = goal.Packet,
                        GoalBy = goal.GoalBy ?? String.Empty,
                        Period = goal.Period,
                        Time = goal.Time,
                        Player = player,
                        Url = path,
                        ReplayData = replayData
                    });
                }

                var fragmentLenght = 1000;

                var count = Math.Ceiling((double)result.Count / (double)fragmentLenght);

                replayData.Min = result.FirstOrDefault().PacketNumber;
                replayData.Max = result.LastOrDefault().PacketNumber;

                replayData.ReplayFragments = new List<ReplayFragment>();

                var fragmentsToAdd = new List<ReplayFragment>();

                for (int i = 0; i < count; i++)
                {
                    var fragment = result.Skip(i * fragmentLenght).Take(fragmentLenght).ToArray();

                    var json = JsonConvert.SerializeObject(fragment);
                    var path = "replayFragments/" + request.Id.ToString() + i.ToString() + ".json";

                    _storageService.UploadTextFile(path, json).Wait();

                    fragmentsToAdd.Add(new ReplayFragment
                    {
                        Data = path,
                        Index = i,
                        Min = fragment.Min(x => x.PacketNumber),
                        Max = fragment.Max(x => x.PacketNumber),
                        ReplayData = replayData
                    });
                }

                foreach(var goal in goalsToAdd)
                {
                    _dbContext.ReplayGoals.Add(goal);
                }

                foreach (var chat in chatsToAdd)
                {
                    _dbContext.ReplayChats.Add(chat);
                }

                foreach (var fragment in fragmentsToAdd)
                {
                    _dbContext.ReplayFragments.Add(fragment);
                }

                _dbContext.SaveChanges();
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

        public async Task<List<StoryViewModel>> GetReplayStories()
        {
            var result = new List<StoryViewModel>();

            var checkDate = DateTime.UtcNow.Date.AddDays(-1);
            var games = await _dbContext.ReplayData.Where(x => x.CreatedOn >= checkDate).Include(x => x.ReplayGoals).ThenInclude(x => x.Player).OrderByDescending(x=>x.CreatedOn).ToListAsync();

            var players = games.SelectMany(x => x.ReplayGoals.Select(x => x.Player)).Distinct().ToList();
            var goals = games.SelectMany(x => x.ReplayGoals).ToList();

            foreach(var player in players)
            {
                var playerGoals = goals.Where(x => x.Player == player).Select(x => x.Id).ToList();

                result.Add(new StoryViewModel
                {
                    PlayerId = player.Id,
                    Name = player.Name,
                    GoalIds = playerGoals
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
    }
}
