using Hangfire;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.DTO;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class ServerService : IServerService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        private IEventService _eventService;
        private readonly IHubContext<ActionHub> _hubContext;
        public ServerService(RankedDb dbContext, ISeasonService seasonService, IEventService eventService, IHubContext<ActionHub> hubContext)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
            _eventService = eventService;
            _hubContext = hubContext;
        }

        public async Task<List<ActiveServerViewModel>> GetActiveServers()
        {
            var servers = await _dbContext.Servers
                .Select(x => new ActiveServerViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    LoggedIn = x.LoggedIn,
                    TeamMax = x.TeamMax,
                    State = x.State,
                    Time = x.Time,
                    Period = x.Period,
                    RedScore = x.RedScore,
                    BlueScore = x.BlueScore,
                })
                .ToListAsync();

            return servers;
        }

        public async Task<ServerLoginViewModel> ServerLogin(ServerLoginRequest request)
        {
            var password = Encryption.GetMD5Hash(request.Password.Trim());
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.ServerToken);
            if (server != null)
            {
                var player = await _dbContext.Players.Include(x=>x.Bans).SingleOrDefaultAsync(x => x.Name == request.Login.Trim() && x.Password == password);
                if (player != null)
                {
                    if (player.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow))
                    {
                        return new ServerLoginViewModel
                        {
                            Id = 0,
                            Success = false,
                            ErrorMessage = "[Server] You are banned"
                        };
                    }
                    else
                    {
                        var approveRequired = _dbContext.Settings.FirstOrDefault().NewPlayerApproveRequired;
                        if ((approveRequired && player.IsApproved) || !approveRequired)
                        {

                            return new ServerLoginViewModel
                            {
                                Id = player.Id,
                                Success = true,
                            };
                        }
                        else
                        {
                            return new ServerLoginViewModel
                            {
                                Id = 0,
                                Success = false,
                                ErrorMessage = "[Server] You are not approved by admin"
                            };
                        }
                    }
                }
                else
                {
                    return new ServerLoginViewModel
                    {
                        Id = 0,
                        Success = false,
                        ErrorMessage = "[Server] Incorrect login or password"
                    };
                }
            }
            else
            {
                return new ServerLoginViewModel
                {
                    Id = 0,
                    Success = false,
                    ErrorMessage = "[Server] Server token wasn't found"
                };
            }
        }

        public async Task<StartGameViewModel> StartGame(StartGameRequest request)
        {
            var result = new StartGameViewModel();

            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                server.TeamMax = request.MaxCount;

                var nextGameCheckGames = _dbContext.Settings.FirstOrDefault().NextGameCheckGames;

                var lastGames = await _dbContext.Games.Include(x => x.GamePlayers).OrderByDescending(x => x.CreatedOn).Take(nextGameCheckGames).SelectMany(x=>x.GamePlayers).ToListAsync();

                var count = request.MaxCount * 2;

                foreach (var player in request.PlayerIds)
                {
                    result.Players.Add(new StartGamePlayerViewModel
                    {
                        Id = player,
                        Score = await _seasonService.GetPlayerElo(player),
                        Count = lastGames.Where(x=>x.PlayerId == player).Count(),
                    });
                }

                result.Players = result.Players.OrderBy(x=>x.Count).Take(count).ToList();

                result.Players = result.Players.OrderByDescending(x => x.Score).ToList();
                result.CaptainRed = result.Players[1].Id;
                result.CaptainBlue = result.Players[0].Id;

                var newId = Guid.NewGuid();
                await _dbContext.Games.AddAsync(new Game
                {
                    Id = newId,
                    RedScore = 0,
                    BlueScore = 0,
                    Season = await _seasonService.GetCurrentSeason(),
                    State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Pick"),
                    MvpId = result.CaptainRed,
                    GamePlayers = randomPlayers.Select(x => new GamePlayer
                    {
                        PlayerId = x,
                        Team = result.CaptainRed == x? 0: (result.CaptainBlue == x? 1: -1),
                        Score = 0,
                        Ping = 0,
                        Ip = String.Empty,
                        Goals = 0,
                        Assists = 0,
                        IsCaptain = result.CaptainRed == x || result.CaptainBlue == x
                    }).ToList()
                });
                await _dbContext.SaveChangesAsync();

                result.GameId= newId;
            }

            return result;
        }

        public async Task Pick(PickRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var game = await _dbContext.Games.Include(x=>x.GamePlayers).FirstOrDefaultAsync(x => x.Id == request.GameId);
                if (game != null)
                {
                    var player = game.GamePlayers.FirstOrDefault(x => x.PlayerId == request.PlayerId);
                    if (player != null)
                    {
                        player.Team = request.Team;

                        if (game.GamePlayers.Where(x => x.Team == -1).Count() == 1)
                        {
                            var team = game.GamePlayers.Where(x => x.Team == 0).Count() > game.GamePlayers.Where(x => x.Team == 1).Count() ? 1 : 0;
                            var lastPlayer = game.GamePlayers.FirstOrDefault(x => x.Team == -1);
                            lastPlayer.Team = team;
                        }

                        if (!game.GamePlayers.Any(x=>x.Team == -1))
                        {
                            game.State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Live");
                        }

                        await _dbContext.SaveChangesAsync();
                    }

                }
            }
        }

        public async Task AddGoal(AddGoalRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var game = await _dbContext.Games.Include(x => x.GamePlayers).FirstOrDefaultAsync(x => x.Id == request.GameId);
                if (game != null)
                {
                    var scorer = game.GamePlayers.FirstOrDefault(x => x.PlayerId == request.Scorer);
                    if (scorer != null)
                    {
                        scorer.Goals += 1;
                    }

                    var assist = game.GamePlayers.FirstOrDefault(x => x.PlayerId == request.Assist);
                    if (assist != null)
                    {
                        assist.Assists += 1;
                    }

                    if (request.Team == 0)
                    {
                        game.RedScore += 1;
                    }
                    else if (request.Team == 1)
                    {
                        game.BlueScore += 1;
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<SaveGameViewModel> SaveGame(SaveGameRequest request)
        {
            var result = new SaveGameViewModel();

            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var game = await _dbContext.Games.Include(x => x.GamePlayers).ThenInclude(x=>x.Player).FirstOrDefaultAsync(x => x.Id == request.GameId);
                if (game != null)
                {
                    game.State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");

                    var winTeam = game.RedScore > game.BlueScore ? 0 : 1;

                    var players = new List<EloCalcPlayerModel>();
                    foreach(var player in game.GamePlayers)
                    {
                        players.Add(new EloCalcPlayerModel
                        {
                            Id = player.PlayerId,
                            Goals = player.Goals,
                            Assists = player.Assists,
                            Points = player.Goals + player.Assists,
                            Performance = 0,
                            RawScore = 0,
                            Team = player.Team,
                            Elo = await _seasonService.GetPlayerElo(player.PlayerId)
                        });
                    }

                    var calculatedElo = RatingCalcHelper.CalcRating(new Models.DTO.EloCalcModel
                    {
                        RedScore = game.RedScore,
                        BlueScore = game.BlueScore,
                        Players = players
                    });

                    foreach (var player in game.GamePlayers)
                    {
                        var calcElo = calculatedElo.Players.FirstOrDefault(x => x.Id == player.PlayerId);
                        if (calcElo != null)
                        {
                            player.Score = calcElo.Elo;
                        }
                    }

                    var mvp = game.GamePlayers.Where(x => x.Team == winTeam).OrderByDescending(x=>x.Goals + x.Assists).FirstOrDefault();
                    game.Mvp = mvp.Player;
                    result.Mvp = mvp.Player.Name;

                    await _dbContext.SaveChangesAsync();

                    var currentSeason = await _seasonService.GetCurrentSeason();
                    var stats = await _seasonService.GetSeasonStats(new CurrentSeasonStatsRequest
                    {
                        SeasonId = currentSeason.Id,
                    });

                    foreach (var gp in game.GamePlayers)
                    {
                        var pos = 1;
                        var total = 0;
                        var playerStat = stats.FirstOrDefault(x=>x.PlayerId == gp.PlayerId);
                        if (playerStat != null)
                        {
                            total = playerStat.Rating;

                            pos = stats.IndexOf(playerStat) + 1;
                        }

                        result.Players.Add(new SaveGamePlayerViewModel
                        {
                            Id = gp.PlayerId,
                            Score = gp.Score,
                            Total = total,
                            Pos = pos
                        });
                    }

                    BackgroundJob.Enqueue(()=>_eventService.CalculateEvents());
                }
            }

            return result;
        }
        public async Task Heartbeat(HeartbeatRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                server.Name = request.Name;
                server.State = request.State;
                server.Time = request.Time;
                server.LoggedIn = request.LoggedIn;
                server.Period = request.Period;
                server.RedScore = request.RedScore;
                server.BlueScore = request.BlueScore;
                server.TeamMax = request.TeamMax;

                await _dbContext.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("onHeartbeat", new HeartbeatSignalrViewModel
                {
                    Id = server.Id,
                    Name = server.Name,
                    RedScore = server.RedScore,
                    BlueScore = server.BlueScore,
                    LoggedIn = server.LoggedIn,
                    Period = server.Period,
                    State = server.State,
                    TeamMax = server.TeamMax,
                    Time = server.Time,
                });
            }
        }
    }
}
