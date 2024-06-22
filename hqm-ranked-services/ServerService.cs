using Hangfire;
using hqm_ranked_backend.Common;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.DTO;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace hqm_ranked_backend.Services
{
    public class ServerService : IServerService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        private IEventService _eventService;
        private INotificationService _notificationService;
        private readonly IHubContext<ActionHub> _hubContext;
        private IMemoryCache _cache;
        private ITeamsService _teamsService;
        public ServerService(RankedDb dbContext, ISeasonService seasonService, IEventService eventService, IHubContext<ActionHub> hubContext, INotificationService notificationService, IMemoryCache memoryCache, ITeamsService teamsService)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
            _eventService = eventService;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _cache = memoryCache;
            _teamsService = teamsService;
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
                    InstanceType =x.InstanceType
                })
                .ToListAsync();

            return servers;
        }

        public async Task<ServerLoginViewModel> ServerLogin(ServerLoginRequest request)
        {
            _cache.TryGetValue("loginDenies", out List<LoginTry> denies);
            if (denies == null)
            {
                denies = new List<LoginTry>();
            }

            _cache.TryGetValue("loginTries", out List<LoginTry> tries);
            if (tries == null)
            {
                tries = new List<LoginTry>();
            }

            if (denies.Any(x => x.Date > DateTime.UtcNow && x.Name == request.Login))
            {
                return new ServerLoginViewModel
                {
                    Id = 0,
                    Success = false,
                    ErrorMessage = "[Server] Login blocked for 10s"
                };
            }
            else
            {
                tries.Add(new LoginTry
                {
                    Name = request.Login,
                    Date = DateTime.UtcNow
                });
                _cache.Set("loginTries", tries);

                var dateToCheck = DateTime.UtcNow.AddSeconds(-2);

                if (tries.Count(x => x.Date > dateToCheck && x.Name == request.Login) >= 3)
                {
                    denies.Add(new LoginTry
                    {
                        Name = request.Login,
                        Date = DateTime.UtcNow.AddSeconds(10)
                    });

                    _cache.Set("loginDenies", denies);

                    return new ServerLoginViewModel
                    {
                        Id = -1,
                        Success = false,
                        ErrorMessage = String.Empty,
                        SendToAll = true
                    };
                }
                else
                {
                    var password = Encryption.GetMD5Hash(request.Password.Trim());
                    var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.ServerToken);
                    if (server != null)
                    {
                        var player = await _dbContext.Players.Include(x => x.Bans).Include(x => x.NicknameChanges).SingleOrDefaultAsync(x => x.Name == request.Login.Trim() && x.Password == password);
                        if (player != null)
                        {
                            var lastBan = player.Bans.Where(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow).OrderByDescending(x=>x.CreatedOn).FirstOrDefault();
                            if (lastBan !=null)
                            {
                                var banUntil = lastBan.CreatedOn.AddDays(lastBan.Days);
                                var banUntilString = banUntil.ToString("d MMMM hh:mm");

                                return new ServerLoginViewModel
                                {
                                    Id = 0,
                                    Success = false,
                                    ErrorMessage = "[Server] You are banned until " + banUntilString+" (UTC)"
                                };
                            }
                            else
                            {
                                var approveRequired = _dbContext.Settings.FirstOrDefault().NewPlayerApproveRequired;
                                
                                if (!player.IsAcceptedRules)
                                {
                                    return new ServerLoginViewModel
                                    {
                                        Id = 0,
                                        Success = false,
                                        ErrorMessage = "[Server] Accept rules on website to log in"
                                    };
                                }

                                if ((approveRequired && player.IsApproved) || !approveRequired)
                                {
                                    var oldNickname = String.Empty;
                                    var oldNicknameItem = player.NicknameChanges.OrderByDescending(x => x.CreatedOn).FirstOrDefault(x => x.CreatedOn.AddDays(30) > DateTime.UtcNow);
                                    if (oldNicknameItem != null)
                                    {
                                        oldNickname = oldNicknameItem.OldNickname;
                                    }

                                    var instanceType = await GetServerType(request.ServerToken);

                                    if (instanceType == InstanceType.Ranked)
                                    {
                                        return new ServerLoginViewModel
                                        {
                                            Id = player.Id,
                                            Success = true,
                                            OldNickname = oldNickname,
                                        };
                                    } 
                                    else if (instanceType == InstanceType.Teams)
                                    {
                                        var teamsState = await _teamsService.GetTeamsState(player.Id);

                                        if (teamsState.Team != null)
                                        {
                                            var currentDate = DateTime.UtcNow;
                                            var incomingGame = await _dbContext.Games
                                                .Include(x => x.GamePlayers)
                                                .ThenInclude(x => x.Player)
                                                .Include(x => x.GameInvites)
                                                .Where(x =>
                                                    x.InstanceType == InstanceType.Teams &&
                                                    x.GameInvites.FirstOrDefault().Date.AddMinutes(-30) < currentDate &&
                                                    x.GameInvites.FirstOrDefault().Date.AddMinutes(30) > currentDate  &&
                                                    (x.RedTeamId == teamsState.Team.Id || x.BlueTeamId == teamsState.Team.Id)
                                                   )
                                                .OrderBy(x => x.GameInvites.FirstOrDefault().Date)
                                                .FirstOrDefaultAsync();

                                            if (incomingGame != null)
                                            {
                                                var team = incomingGame.RedTeamId == teamsState.Team.Id ? 0 : 1;
                                                return new ServerLoginViewModel
                                                {
                                                    Id = player.Id,
                                                    Success = true,
                                                    OldNickname = oldNickname,
                                                    Team = team
                                                };
                                            }
                                            else
                                            {
                                                return new ServerLoginViewModel
                                                {
                                                    Id = 0,
                                                    Success = false,
                                                    ErrorMessage = "[Server] You will not have any games for the next 30 minutes"
                                                };
                                            }
                                        }
                                        else
                                        {
                                            return new ServerLoginViewModel
                                            {
                                                Id = 0,
                                                Success = false,
                                                ErrorMessage = "[Server] You are not in team"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        return new ServerLoginViewModel
                                        {
                                            Id = 0,
                                            Success = false,
                                            ErrorMessage = "[Server] Server instance type not supported"
                                        };
                                    }

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
                            var pl = await _dbContext.Players.SingleOrDefaultAsync(x => x.Name == request.Login.Trim());
                            return new ServerLoginViewModel
                            {
                                Id = pl != null ? pl.Id : -1,
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
            }
        }

        public async Task<StartGameViewModel> StartGame(StartGameRequest request)
        {
            var result = new StartGameViewModel();

            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                server.TeamMax = request.MaxCount;
                var instanceType = await GetServerType(request.Token);
                if (instanceType == InstanceType.Ranked)
                {
                    var nextGameCheckGames = _dbContext.Settings.FirstOrDefault().NextGameCheckGames;
                    var shadowBanReportsCount = _dbContext.Settings.FirstOrDefault().ShadowBanReportsCount;

                    var lastGames = await _dbContext.Games.Include(x => x.GamePlayers).OrderByDescending(x => x.CreatedOn).Take(nextGameCheckGames).SelectMany(x => x.GamePlayers).ToListAsync();

                    var count = request.MaxCount * 2;

                    var date = DateTime.UtcNow;

                    foreach (var player in request.PlayerIds)
                    {
                        var reportsCount = await _dbContext.Reports.Include(x => x.To).Where(x => x.CreatedOn.AddMonths(1) > date && x.To.Id == player).CountAsync();
                        if (reportsCount < 3)
                        {
                            reportsCount = 0;
                        }

                        result.Players.Add(new StartGamePlayerViewModel
                        {
                            Id = player,
                            Score = await _seasonService.GetPlayerElo(player),
                            Count = lastGames.Where(x => x.PlayerId == player).Count(),
                            Reports = reportsCount
                        });
                    }

                    result.Players = result.Players.OrderBy(x => x.Reports).ThenBy(x => x.Count).Take(count).ToList();

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
                        GamePlayers = result.Players.Select(x => new GamePlayer
                        {
                            PlayerId = x.Id,
                            Team = result.CaptainRed == x.Id ? 0 : (result.CaptainBlue == x.Id ? 1 : -1),
                            Score = 0,
                            Ping = 0,
                            Ip = String.Empty,
                            Goals = 0,
                            Assists = 0,
                            IsCaptain = result.CaptainRed == x.Id || result.CaptainBlue == x.Id
                        }).ToList()
                    });
                    await _dbContext.SaveChangesAsync();

                    result.GameId = newId;
                }
                else if (instanceType == InstanceType.Teams)
                {
                    var currentDate = DateTime.UtcNow;

                    var season = await _seasonService.GetCurrentSeason();
                    var teamsPlayers = await _dbContext.TeamPlayers.Include(x=>x.Team).Include(x => x.Player).Where(x => request.PlayerIds.Contains(x.Player.Id) && x.Team.Season == season).ToListAsync();
                    var teamIds = teamsPlayers.Select(x => x.Team.Id as Guid?).Distinct().ToList();

                    var incomingGame = await _dbContext.Games
                        .Include(x => x.GamePlayers)
                        .ThenInclude(x => x.Player)
                        .Include(x => x.GameInvites)
                        .Where(x =>
                            x.InstanceType == InstanceType.Teams &&
                            x.GameInvites.FirstOrDefault().Date.AddMinutes(-30) < currentDate &&
                            x.GameInvites.FirstOrDefault().Date.AddMinutes(30) > currentDate &&
                            (teamIds.Contains(x.RedTeamId) && teamIds.Contains(x.BlueTeamId))
                           )
                        .OrderBy(x => x.GameInvites.FirstOrDefault().Date)
                        .FirstOrDefaultAsync();

                    if (incomingGame != null)
                    {
                        incomingGame.State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Live");

                        var gamePlayers = new List<GamePlayer>();

                        var redTeamPlayers = incomingGame.RedTeam.TeamPlayers.Select(x=>x.Player.Id).ToList();

                        foreach (var teamPlayer in teamsPlayers)
                        {
                            gamePlayers.Add(new GamePlayer
                            {
                                PlayerId = teamPlayer.Player.Id,
                                Team = redTeamPlayers.Contains(teamPlayer.Player.Id)? 0: 1,
                                Score = 0,
                                Ping = 0,
                                Ip = String.Empty,
                                Goals = 0,
                                Assists = 0,
                                IsCaptain = false
                            });
                        }

                        incomingGame.GamePlayers = gamePlayers;
                        await _dbContext.SaveChangesAsync();

                        foreach (var player in request.PlayerIds)
                        {
                            result.Players.Add(new StartGamePlayerViewModel
                            {
                                Id = player,
                                Score = await _seasonService.GetPlayerElo(player),
                                Count = 0,
                                Reports = 0
                            });
                        }
                        result.GameId = incomingGame.Id;
                    }
                }

                await _notificationService.SendDiscordStartGameNotification(server.Name);

                var playersIds = result.Players.Select(x => x.Id).ToList();
                var tokens = await _dbContext.PlayerNotifications
                        .Include(x => x.Player)
                         .Where(x => !String.IsNullOrEmpty(x.Token))
                        .Where(x => x.GameStarted == NotifyType.Enabled || (x.GameStarted == NotifyType.EnabledWithMe && playersIds.Contains(x.Player.Id)))
                        .Select(x => x.Token)
                        .ToListAsync();
                await _notificationService.SendPush(server.Name, String.Format("Game started"), tokens);
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
                    if (game.State != await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned"))
                    {
                        game.State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
                    }

                    game.RedScore = request.RedScore;
                    game.BlueScore = request.BlueScore;

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

                    var startingElo = _dbContext.Settings.FirstOrDefault().StartingElo;
                    foreach (var gp in game.GamePlayers)
                    {
                        if (!await _dbContext.Elos.AnyAsync(x => x.Player == gp.Player && x.Season == game.Season))
                        {
                            _dbContext.Elos.Add(new Elo
                            {
                                Player = gp.Player,
                                Season = game.Season,
                                Value = startingElo
                            });
                        }
                    }

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

                    await _notificationService.SendDiscordEndGameNotification(server.Name);

                    var playersIds = game.GamePlayers.Select(x => x.PlayerId).ToList();
                    var tokens = await _dbContext.PlayerNotifications
                        .Include(x => x.Player)
                         .Where(x =>!String.IsNullOrEmpty(x.Token))
                        .Where(x => x.GameEnded == NotifyType.Enabled || (x.GameEnded == NotifyType.EnabledWithMe && playersIds.Contains(x.Player.Id)))
                        .Select(x => x.Token)
                        .ToListAsync();

                    await _notificationService.SendPush(server.Name, String.Format("Game ended"), tokens);
                }
            }

            return result;
        }
        public async Task Heartbeat(HeartbeatRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var isLoggedChanged = server.LoggedIn != request.LoggedIn;

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
                    InstanceType = server.InstanceType
                });

                if (isLoggedChanged && server.State == 0)
                {
                    await _notificationService.SendDiscordNotification(server.Name, server.LoggedIn, server.TeamMax);

                    var tokens = await _dbContext.PlayerNotifications
                                .Include(x => x.Player)
                                 .Where(x => !String.IsNullOrEmpty(x.Token))
                                .Where(x => x.LogsCount <= server.LoggedIn)
                                .Select(x => x.Token)
                                .ToListAsync();
                    await _notificationService.SendPush(server.Name, String.Format("Logged in {0}/{1}", server.LoggedIn, server.TeamMax * 2), tokens);
                }
            }
        }

        public async Task<ReportViewModel> Report(ReportRequest request)
        {
            var result = new ReportViewModel();

            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var gamesCount = await _dbContext.GamePlayers.Where(x=>x.PlayerId == request.FromId).CountAsync();
                if (gamesCount < 20)
                {
                    result.Message = "[Server] You can't submit a report until you have played 20 games";
                }
                else
                {
                    var weekBefore = DateTime.UtcNow.AddDays(-7);
                    var reportedBefore = await _dbContext.Reports.Include(x => x.From).Include(x => x.To).Where(x => x.From.Id == request.FromId && x.To.Id == request.ToId && x.CreatedOn > weekBefore).ToListAsync();
                    if (reportedBefore.Any())
                    {
                        result.Message = "[Server] You can report this player once per week";
                    }
                    else
                    {
                        var reasons = await _dbContext.Rules.OrderBy(x => x.CreatedOn).ToListAsync();
                        var reason = reasons[request.ReasonIndex - 1];
                        if (reason != null)
                        {
                            var fromPlayer = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == request.FromId);
                            var toPlayer = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == request.ToId);

                            _dbContext.Reports.Add(new Reports
                            {
                                From = fromPlayer,
                                To = toPlayer,
                            });
                            await _dbContext.SaveChangesAsync();

                            result.Message = String.Format("[Server] Somebody reported {0}, reason: {1}", toPlayer.Name, reason.Title);
                            result.Success = true;
                        }
                        else
                        {
                            result.Message = "[Server] Please specify the correct reason";
                        }
                    }
                }
            }

            return result;
        }

        public async Task Reset(ResetRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var game = await _dbContext.Games.Include(x => x.GamePlayers).ThenInclude(x => x.Player).FirstOrDefaultAsync(x => x.Id == request.GameId);
                if (game != null)
                {
                    game.State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Canceled");
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task Resign(ResignRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x => x.Token == request.Token);
            if (server != null)
            {
                var game = await _dbContext.Games.Include(x => x.GamePlayers).ThenInclude(x => x.Player).FirstOrDefaultAsync(x => x.Id == request.GameId);
                if (game != null)
                {
                    game.State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");
                    if (request.Team == 0)
                    {
                        game.BlueScore = game.RedScore + 7;
                    }
                    else
                    {
                        game.RedScore = game.BlueScore + 7;
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<string>> GetReasons()
        {
            var reasons = await _dbContext.Rules.OrderBy(x => x.CreatedOn).Select(x=>x.Title).ToListAsync();

            return reasons;
        }

        public async Task<InstanceType> GetServerType(string token)
        {
            var server = await _dbContext.Servers.FirstOrDefaultAsync(x => x.Token == token);
            return server != null ? server.InstanceType : InstanceType.Ranked;
        }
    }
}
