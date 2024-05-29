﻿using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace hqm_ranked_backend.Services
{
    public class ReplayCalcService: IReplayCalcService
    {
        private RankedDb _dbContext;
        private IStorageService _storageService;
        public ReplayCalcService(RankedDb dbContext, IStorageService storageService)
        {
            _dbContext = dbContext;
            _storageService = storageService;
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
                try
                {
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

                    var goalTaskList = new List<Task>();

                    foreach (var goal in processedData.Goals)
                    {
                        var id = Guid.NewGuid();

                        var goalTicks = result.Skip((int)(goal.Packet - 250)).Take(300).ToList();

                        var json = JsonConvert.SerializeObject(goalTicks);
                        var path = "replayGoals/" + request.Id.ToString() + id.ToString() + ".json";

                        goalTaskList.Add(Task.Run(() => _storageService.UploadTextFile(path, json).Wait()));

                        var player = _dbContext.Players.FirstOrDefault(x => x.Name == goal.GoalBy);
                        if (player != null)
                        {
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
                    }
                    var t = Task.WhenAll(goalTaskList);
                    t.Wait();

                    var highlightsToAdd = new List<ReplayHighlight>();

                    foreach (var shot in processedData.Shots)
                    {
                        var player = _dbContext.Players.FirstOrDefault(x => x.Name == shot.Name);

                        highlightsToAdd.Add(new ReplayHighlight
                        {
                            Packet = shot.Packet,
                            Type = HighlightType.Shot,
                            ReplayData = replayData,
                            Name = shot.Name,
                            Player = player,
                            Url = String.Empty
                        });
                    }


                    foreach (var save in processedData.Saves)
                    {
                        var player = _dbContext.Players.FirstOrDefault(x => x.Name == save.Name);

                        highlightsToAdd.Add(new ReplayHighlight
                        {
                            Packet = save.Packet,
                            Type = HighlightType.Save,
                            ReplayData = replayData,
                            Name = save.Name,
                            Player = player,
                            Url = String.Empty
                        });
                    }

                    var fragmentLenght = 1000;

                    var count = Math.Ceiling((double)result.Count / (double)fragmentLenght);

                    replayData.Min = result.FirstOrDefault().PacketNumber;
                    replayData.Max = result.LastOrDefault().PacketNumber;

                    replayData.ReplayFragments = new List<ReplayFragment>();

                    var fragmentsTaskList = new List<Task>();
                    var fragmentsToAdd = new List<ReplayFragment>();

                    for (int i = 0; i < count; i++)
                    {
                        var fragment = result.Skip(i * fragmentLenght).Take(fragmentLenght).ToArray();

                        var json = JsonConvert.SerializeObject(fragment);
                        var path = "replayFragments/" + request.Id.ToString() + i.ToString() + ".json";

                        fragmentsTaskList.Add(Task.Run(() => _storageService.UploadTextFile(path, json).Wait()));

                        fragmentsToAdd.Add(new ReplayFragment
                        {
                            Data = path,
                            Index = i,
                            Min = fragment.Min(x => x.PacketNumber),
                            Max = fragment.Max(x => x.PacketNumber),
                            ReplayData = replayData
                        });
                    }

                    var t2 = Task.WhenAll(fragmentsTaskList);
                    t2.Wait();

                    var wasAdded = false;
                    var replayDataItem = _dbContext.ReplayData.Include(x => x.Game).Include(x => x.ReplayFragments).FirstOrDefault(x => x.Game.Id == request.Id);

                    if (replayDataItem != null)
                    {
                        wasAdded = replayDataItem.ReplayFragments.Any();
                    }

                    if (!wasAdded)
                    {
                        _dbContext.ReplayGoals.AddRange(goalsToAdd);
                        _dbContext.ReplayChats.AddRange(chatsToAdd);
                        _dbContext.ReplayFragments.AddRange(fragmentsToAdd);
                        _dbContext.ReplayHighlights.AddRange(highlightsToAdd);
                    }

                    var game = _dbContext.Games.Include(x => x.GamePlayers).ThenInclude(x => x.Player).FirstOrDefault(x => x.Id == replayData.Game.Id);
                    if (game != null)
                    {
                        var sum = processedData.Possession.Sum(x => x.Touches);

                        foreach (var player in game.GamePlayers)
                        {
                            var foundPossession = processedData.Possession.FirstOrDefault(x => x.Name == player.Player.Name);
                            if (foundPossession != null)
                            {
                                player.Possession = (double)foundPossession.Touches / (double)sum * 100;
                            }

                            player.Shots = processedData.Shots.Count(x => x.Name == player.Player.Name);
                            player.Saves = processedData.Saves.Count(x => x.Name == player.Player.Name);

                            player.Conceded = 0;

                            var goaliePositions = processedData.Goalies.Where(x => x.Name == player.Player.Name).ToList();
                            foreach (var gp in goaliePositions)
                            {
                                var conceded = 0;

                                foreach (var goal in goalsToAdd)
                                {
                                    var scorer = game.GamePlayers.FirstOrDefault(x => x.Player == goal.Player);
                                    if (player.Team != scorer.Team)
                                    {
                                        if (goal.Packet > gp.StartPacket && goal.Packet < gp.EndPacket)
                                        {
                                            conceded++;
                                        }
                                    }
                                }

                                player.Conceded = conceded;
                            }
                        }
                    }

                    _dbContext.SaveChanges();
                }
                catch
                {
                    _dbContext.ReplayData.Remove(replayData);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
