using Google.Apis.Auth.OAuth2;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.DTO;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SixLabors.ImageSharp;
using SpotifyAPI.Web.Http;
using System;
using System.ComponentModel;
using TimeAgo;

namespace hqm_ranked_backend.Services
{
    public class TeamsService : ITeamsService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        private IImageGeneratorService _imageGeneratorService;
        private IStorageService _storageService;
        private INotificationService _notificationService;
        private readonly IHubContext<ActionHub> _hubContext;
        public TeamsService(RankedDb dbContext, ISeasonService seasonService, IImageGeneratorService imageGeneratorService, IStorageService storageService, INotificationService notificationService, IHubContext<ActionHub> hubContext)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
            _imageGeneratorService = imageGeneratorService;
            _storageService = storageService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }
        public async Task<TeamsStateViewModel> GetTeamsState(int? userId)
        {
            var result = new TeamsStateViewModel();

            if (userId != null)
            {
                var currentSeason = await _seasonService.GetCurrentSeason();
                var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
                if (player != null)
                {
                    result.Cost = player.Cost != null ? player.Cost.Cost : 0;

                    var teamPlayer = await _dbContext.TeamPlayers.Include(x => x.Team).ThenInclude(x => x.Budgets).ThenInclude(x => x.InvitedPlayer).Include(x=>x.Team).ThenInclude(x=>x.TeamPlayers).ThenInclude(x=>x.Player).ThenInclude(x=>x.Cost).FirstOrDefaultAsync(x => x.Player == player);

                    if (teamPlayer != null)
                    {
                        result.IsCaptain = teamPlayer.Team.Captain == player;
                        result.IsAssistant = teamPlayer.Team.Assistant == player;
                        result.CaptainId = teamPlayer.Team.Captain !=null? teamPlayer.Team.Captain.Id: null;
                        result.AssistantId = teamPlayer.Team.Assistant != null ? teamPlayer.Team.Assistant.Id : null;
                        result.TeamsMaxPlayers = _dbContext.Settings.FirstOrDefault().TeamsMaxPlayer;
                        

                        result.Team = new CurrentTeamViewModel
                        {
                            Id = teamPlayer.Team.Id,
                            Name = teamPlayer.Team.Name,
                            Budget = teamPlayer.Team.Budgets.Sum(x => x.Change),
                            Players = teamPlayer.Team.TeamPlayers.Select(x => new CurrentTeamPlayerViewModel
                            {
                                Id = x.Player.Id,
                                Name = x.Player.Name,
                                Cost = x.Player.Cost.Cost
                            }).ToList(),
                            BudgetHistory = teamPlayer.Team.Budgets.OrderByDescending(x => x.CreatedOn).Select(x => new CurrentTeamBudgetViewModel
                            {
                                Date = x.CreatedOn,
                                Change = x.Change,
                                Type = x.Type,
                                InvitedPlayerId = x.InvitedPlayer != null ? x.InvitedPlayer.Id : null,
                                InvitedPlayerNickname = x.InvitedPlayer != null ? x.InvitedPlayer.Name : null,
                            }).ToList()
                        };
                    }
                    else
                    {
                        result.CanCreateTeam = player.Cost != null;
                    }
                }
            }

            return result;
        }

        public async Task<TeamViewModel> GetTeam(Guid teamId)
        {
            var result = await _dbContext.Teams.Include(x => x.TeamPlayers).ThenInclude(x => x.Player).Include(x => x.Captain).Include(x => x.Assistant).Include(x => x.Budgets).Select(x => new TeamViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CaptainId = x.Captain != null ? x.Captain.Id : null,
                AssistantId = x.Assistant != null ? x.Assistant.Id : null,
                Players = x.TeamPlayers.Select(y => new TeamPlayerViewModel
                {
                    Id = y.Player.Id,
                    Name = y.Player.Name,
                }).OrderBy(y=>y.Id !=x.Captain.Id).ThenBy(y=>y.Id != x.Assistant.Id).ToList(),
                BudgetHistory = x.Budgets.OrderByDescending(x => x.CreatedOn).Select(x => new TeamBudgetViewModel
                {
                    Date = x.CreatedOn,
                    Change = x.Change,
                    Type = x.Type,
                    InvitedPlayerId = x.InvitedPlayer != null ? x.InvitedPlayer.Id : null,
                    InvitedPlayerNickname = x.InvitedPlayer != null ? x.InvitedPlayer.Name : null,
                }).ToList()
            }).FirstOrDefaultAsync(x => x.Id == teamId);

            var games = await _dbContext.Games.Include(x=>x.RedTeam).Include(x=>x.BlueTeam).Where(x => x.RedTeamId == teamId || x.BlueTeamId == teamId).ToListAsync();

            result.Games = games.Count;
            result.Goals = games.Sum(x=>x.RedTeamId == teamId? x.RedScore : x.BlueScore);

            return result;
        }

        public async Task CreateTeam(string name, int userId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
            if (state.CanCreateTeam && player != null)
            {
                var currentSeason = await _seasonService.GetCurrentSeason();

                var teamEntity = _dbContext.Teams.Add(new Team
                {
                    Name = name,
                    Season = currentSeason,
                    Captain = player,
                });

                var path = String.Format("images/{0}.png", teamEntity.Entity.Id);
                var file = _imageGeneratorService.GenerateImage();
                var strm = new MemoryStream();
                file.SaveAsPng(strm);

                await _storageService.UploadFileStream(path, strm);

                _dbContext.TeamPlayers.Add(new TeamPlayer
                {
                    Player = player,
                    Team = teamEntity.Entity,
                });

                _dbContext.Budgets.Add(new Budget
                {
                    Team = teamEntity.Entity,
                    Type = BudgetType.Start,
                    Change = 2500000,
                });

                _dbContext.Budgets.Add(new Budget
                {
                    Team = teamEntity.Entity,
                    Type = BudgetType.Invite,
                    InvitedPlayer = player,
                    Change = -player.Cost.Cost,
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UploadAvatar(int userId, IFormFile file)
        {
            if (file != null)
            {
                var state = await GetTeamsState(userId);
                var name = "images/" + state.Team.Id + ".png";
                await _storageService.UploadFile(name, file);
            }
        }

        public async Task LeaveTeam(int userId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
            if (state.Team != null && player != null)
            {
                var team = await _dbContext.Teams.Include(x => x.TeamPlayers).ThenInclude(x => x.Player).FirstOrDefaultAsync(x => x.Id == state.Team.Id);
                if (team != null)
                {
                    if (state.IsCaptain)
                    {
                        if (team.TeamPlayers.Any(x => x.Player.Id != userId))
                        {
                            if (team.Assistant != null)
                            {
                                team.Captain = team.Assistant;
                                team.Assistant = null;
                            }
                            else
                            {
                                var newCap = team.TeamPlayers.FirstOrDefault(x => x.Player.Id != userId);
                                if (newCap != null)
                                {
                                    team.Captain = newCap.Player;
                                }
                            }
                        }
                        else
                        {
                            team.Captain = null;
                        }
                    }

                    if (state.IsAssistant)
                    {
                        team.Assistant = null;
                    }

                    var teamPlayer = team.TeamPlayers.FirstOrDefault(x => x.Player.Id == userId);
                    if (teamPlayer != null)
                    {
                        _dbContext.TeamPlayers.Remove(teamPlayer);
                    }
                    _dbContext.Budgets.Add(new Budget
                    {
                        Team = team,
                        Type = BudgetType.Leave,
                        InvitedPlayer = player,
                        Change = player.Cost.Cost,
                    });

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<FreeAgentViewModel>> GetFreeAgents(int? userId)
        {
            var currentSeason = await _seasonService.GetCurrentSeason();
            var teamPlayersIds = await _dbContext.TeamPlayers.Include(x => x.Team).Include(x => x.Player).Where(x => x.Team.Season == currentSeason).Select(x => x.Player.Id).ToListAsync();

            var invites = new List<CurrentPlayerInvite>();
            if (userId != null)
            {
                var state = await GetTeamsState(userId);
                if (state.IsCaptain || state.IsAssistant)
                {
                    invites = await _dbContext.PlayerInvites.Include(x => x.Team).Include(x => x.Player).Where(x => x.Team.Id == state.Team.Id).Select(x => new CurrentPlayerInvite
                    {
                        InviteId = x.Id,
                        PlayerId = x.Player.Id
                    }).ToListAsync();
                }
            }

            var result = await _dbContext.Players
                .Where(x => !teamPlayersIds.Contains(x.Id) && x.Cost != null)
                .Select(x => new FreeAgentViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Cost = x.Cost.Cost,
                    InviteId = null
                })
                .OrderByDescending(x => x.Cost)
                .ToListAsync();

            foreach(var item in result)
            {
                item.InviteId = invites.Any(y => y.PlayerId == item.Id) ? invites.FirstOrDefault(y => y.PlayerId == item.Id).InviteId : null;
            }

            return result;
        }

        public async Task InvitePlayer(int userId, int invitedId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
            if (player != null && (state.IsCaptain || state.IsAssistant))
            {
                if (!_dbContext.PlayerInvites.Include(x => x.Team).Include(x => x.Player).Any(x => x.Player.Id == invitedId && x.Team.Id == state.Team.Id))
                {
                    var invitedPlayer = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == invitedId);
                    var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == state.Team.Id);
                    if (invitedPlayer != null)
                    {
                        var budgetItem = new Budget
                        {
                            Change = -invitedPlayer.Cost.Cost,
                            InvitedPlayer = invitedPlayer,
                            Team = team,
                            Type = BudgetType.Invite
                        };

                        var budget = _dbContext.Budgets.Add(budgetItem);

                        _dbContext.PlayerInvites.Add(new PlayerInvite
                        {
                            Budget = budget.Entity,
                            Player = invitedPlayer,
                            Team = team
                        });

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task CancelInvite(int userId, Guid inviteId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
            if (player != null && (state.IsCaptain || state.IsAssistant))
            {
                var playerInvite = await _dbContext.PlayerInvites.Include(x => x.Budget).FirstOrDefaultAsync(x => x.Id == inviteId);
                if (playerInvite != null)
                {
                    _dbContext.Budgets.Remove(playerInvite.Budget);
                    _dbContext.PlayerInvites.Remove(playerInvite);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<PlayerInviteViewModel>> GetInvites(int userId)
        {
            var result = new List<PlayerInviteViewModel>();

            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.Include(x => x.Cost).FirstOrDefaultAsync(x => x.Id == userId);
            if (player != null && state.Team == null)
            {
                result = await _dbContext.PlayerInvites
                    .Include(x => x.Player)
                    .Include(x => x.Team)
                    .Where(x => x.Player.Id == userId)
                    .Select(x => new PlayerInviteViewModel
                    {
                        InviteId = x.Id,
                        TeamId = x.Team.Id,
                        TeamName = x.Team.Name,
                    }).ToListAsync();
            }

            return result;
        }

        public async Task ApplyPlayerInvite(int userId, Guid inviteId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
            if (player != null && state.Team == null)
            {
                var playerInvite = await _dbContext.PlayerInvites.Include(x=>x.Team).FirstOrDefaultAsync(x => x.Id == inviteId);
                if (playerInvite != null)
                {
                    _dbContext.TeamPlayers.Add(new TeamPlayer
                    {
                        Player = player,
                        Team = playerInvite.Team,
                    });
                    _dbContext.PlayerInvites.Remove(playerInvite);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task DeclinePlayerInvite(int userId, Guid inviteId)
        {
            var state = await GetTeamsState(userId);
            var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
            if (player != null && state.Team == null)
            {
                var playerInvite = await _dbContext.PlayerInvites.Include(x => x.Team).Include(x=>x.Budget).FirstOrDefaultAsync(x => x.Id == inviteId);
                if (playerInvite != null)
                {
                    _dbContext.Budgets.Remove(playerInvite.Budget);
                    _dbContext.PlayerInvites.Remove(playerInvite);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task CancelExpiredInvites()
        {
            var dateDayBefore = DateTime.UtcNow.AddDays(-1);
            var playerInvites = await _dbContext.PlayerInvites.Include(x => x.Budget).Where(x => x.CreatedOn < dateDayBefore).ToListAsync();
            if (playerInvites.Any())
            {
                foreach(var invite in playerInvites)
                {
                    _dbContext.Budgets.Remove(invite.Budget);
                    _dbContext.PlayerInvites.Remove(invite);
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SellPlayer(int userId, int playerId)
        {
            var state = await GetTeamsState(userId);
            if (state.IsCaptain || state.IsAssistant)
            {
                var playerToSell = await _dbContext.TeamPlayers.Include(x => x.Team).Include(x => x.Player).ThenInclude(x => x.Cost).FirstOrDefaultAsync(x => x.Team.Id == state.Team.Id && x.Player.Id == playerId);
                if (playerToSell != null)
                {
                    _dbContext.Budgets.Add(new Budget
                    {
                        Change = playerToSell.Player.Cost.Cost,
                        InvitedPlayer = playerToSell.Player,
                        Team = playerToSell.Team,
                        Type = BudgetType.Sell,
                    });
                    _dbContext.TeamPlayers.Remove(playerToSell);

                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task MakeCaptain(int userId, int playerId)
        {
            var state = await GetTeamsState(userId);
            if (state.IsCaptain)
            {
                var playerToCap = await _dbContext.Players.FirstOrDefaultAsync(x=>x.Id == playerId);
                if (playerToCap != null)
                {
                    var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == state.Team.Id);
                    if (team != null)
                    {
                        team.Captain = playerToCap;
                        
                        if (team.Assistant == playerToCap)
                        {
                            team.Assistant = null;
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task MakeAssistant(int userId, int playerId)
        {
            var state = await GetTeamsState(userId);
            if (state.IsCaptain || state.IsAssistant)
            {
                var playerToAssistant = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == playerId);
                if (playerToAssistant != null)
                {
                    var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == state.Team.Id);
                    if (team != null)
                    {
                        team.Assistant = playerToAssistant;

                        if (team.Captain == playerToAssistant)
                        {
                            team.Captain = null;
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<string> CreateGameInvite(int userId, DateTime date, int count)
        {
            var result = String.Empty;

            var state = await GetTeamsState(userId);
            if (state.IsCaptain || state.IsAssistant)
            {
                var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == state.Team.Id);
                if (team != null)
                {
                    var invites = await GetGameInvites(userId);
                    if (invites.Where(x => x.IsCurrentTeam).Count() < 3)
                    {
                        _dbContext.GameInvites.Add(new GameInvites
                        {
                            Date = date,
                            InvitedTeam = team,
                            GamesCount = count
                        });

                        await _dbContext.SaveChangesAsync();

                        await _hubContext.Clients.All.SendAsync("onInvitesChange");
                    }
                    else
                    {
                        result = "You can't create more than three active invites";
                    }
                }
            }

            return result;
        }

        public async Task RemoveGameInvite(int userId, Guid inviteId)
        {
            var state = await GetTeamsState(userId);
            if (state.IsCaptain || state.IsAssistant)
            {
                var invite = await _dbContext.GameInvites.FirstOrDefaultAsync(x => x.Id == inviteId);
                _dbContext.GameInvites.Remove(invite);

                await _dbContext.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("onInvitesChange");
            }
        }

        public async Task<List<GameInviteViewModel>> GetGameInvites(int userId)
        {
            var result = new List<GameInviteViewModel>();

            var state = await GetTeamsState(userId);
            if (state.Team != null)
            {
                var dateNow = DateTime.UtcNow;

                var teamPlayersIds = state.Team.Players.Select(x=>x.Id).ToList();

                result = await _dbContext.GameInvites
                    .Include(x => x.InvitedTeam)
                    .ThenInclude(x => x.TeamPlayers)
                    .Include(x => x.GameInviteVotes)
                    .ThenInclude(x => x.Player)
                    .Include(x => x.Games)
                    .Where(x => x.Games.Count == 0)
                    .Select(x => new GameInviteViewModel
                    {
                        Id = x.Id,
                        Date = x.Date,
                        IsCurrentTeam = x.InvitedTeam.Id == state.Team.Id,
                        VotesCount = x.GameInviteVotes.Where(y => x.InvitedTeam.TeamPlayers.Any(k => k.Player == y.Player)).Count(),
                        GamesCount = x.GamesCount,
                        Votes = x.GameInviteVotes.Where(x => teamPlayersIds.Contains(x.Player.Id)).Select(x => new GameInviteVoteViewModel
                        {
                            Id = x.Player.Id
                        }).ToList()
                    })
                    .Where(x => x.Date > dateNow)
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                result = result.Where(x =>x.IsCurrentTeam || (!x.IsCurrentTeam && x.VotesCount >= state.TeamsMaxPlayers)).ToList();
            }

            return result;
        }

        public async Task VoteGameInvite(int userId, Guid inviteId)
        {
            var state = await GetTeamsState(userId);
            if (state.Team != null)
            {
                var invite = await _dbContext.GameInvites.Include(x => x.GameInviteVotes).ThenInclude(x=>x.Player).Include(x=>x.InvitedTeam).ThenInclude(x=>x.TeamPlayers).ThenInclude(x=>x.Player).FirstOrDefaultAsync(x => x.Id == inviteId);
                var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);
                if (invite != null && player !=null)
                {
                    var vote = invite.GameInviteVotes.FirstOrDefault(x => x.Player == player);
                    if (vote != null)
                    {
                        invite.GameInviteVotes.Remove(vote);
                    }
                    else
                    {
                        invite.GameInviteVotes.Add(new GameInviteVote
                        {
                             Player = player
                        });

                        var invitedVotesCount = invite.GameInviteVotes.Where(y => invite.InvitedTeam.TeamPlayers.Any(k => k.Player == y.Player)).Count();

                        if (invitedVotesCount >= state.TeamsMaxPlayers && !invite.NotificationSent)
                        {
                            await _notificationService.SendDiscordTeamInvite(invite);

                            invite.NotificationSent = true;
                        }

                        if (invitedVotesCount >= state.TeamsMaxPlayers && !invite.InvitedTeam.TeamPlayers.Any(k => k.Player == player))
                        {
                            var otherTeamVotes = invite.GameInviteVotes.Where(x=> state.Team.Players.Any(y=>y.Id == x.Player.Id)).Count();
                            if (otherTeamVotes >= state.TeamsMaxPlayers)
                            {
                                var blueTeam = await  _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == state.Team.Id);

                                var gamePlayers = new List<GamePlayer>();

                                invite.Games = new List<Game>();

                                for (int i = 0; i< invite.GamesCount; i++)
                                {
                                    invite.Games.Add(new Game
                                    {
                                        InstanceType = Common.InstanceType.Teams,
                                        Mvp = player,
                                        RedTeam = invite.InvitedTeam,
                                        BlueTeam = blueTeam,
                                        Season = await _seasonService.GetCurrentSeason(),
                                        State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Scheduled"),
                                        CreatedOn = invite.Date,
                                    });
                                }
                                
                                await _dbContext.SaveChangesAsync();

                                await _notificationService.SendDiscordTeamsGame(invite, invite.InvitedTeam.Name, blueTeam.Name);
                                invite.CreatedOn = invite.Date;

                                await _hubContext.Clients.All.SendAsync("onGamesChange");
                            }
                        }
                    }

                    await _dbContext.SaveChangesAsync();

                    await _hubContext.Clients.All.SendAsync("onInvitesChange");
                }
            }
        }

        public async Task<List<TeamsStatsViewModel>> GetTeamsStats(CurrentSeasonStatsRequest request)
        {
            var result = new List<TeamsStatsViewModel>();

            var ended = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Ended");
            var resigned = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Resigned");

            var season = await _dbContext.Seasons.SingleOrDefaultAsync(x => x.Id == request.SeasonId);

            var teams = await _dbContext.Teams.Include(x=>x.TeamPlayers).Where(x=>x.Season == season).ToListAsync();

            foreach(var team in teams)
            {
                if (team.TeamPlayers.Count > 0)
                {
                    var teamGames = await _dbContext.Games.Include(x => x.RedTeam).Include(x => x.BlueTeam).Where(x => (x.RedTeam == team || x.BlueTeam == team) && (x.State == ended || x.State == resigned)).ToListAsync();
                    var redGames = teamGames.Where(x => x.RedTeam == team).ToList();
                    var blueGames = teamGames.Where(x => x.BlueTeam == team).ToList();

                    var teamStats = new TeamsStatsViewModel
                    {
                        Id = team.Id,
                        Name = team.Name,
                        Goals = redGames.Sum(x => x.RedScore) + blueGames.Sum(x => x.BlueScore),
                        GoalsConceded = redGames.Sum(x => x.BlueScore) + blueGames.Sum(x => x.RedScore),
                        Win = redGames.Count(x => x.RedScore > x.BlueScore) + blueGames.Count(x => x.RedScore < x.BlueScore),
                        Lose = redGames.Count(x => x.RedScore < x.BlueScore) + blueGames.Count(x => x.RedScore > x.BlueScore),
                        Place = 0,
                        Rating = redGames.Sum(x => (int)x.RedPoints) + blueGames.Sum(x => (int)x.BluePoints)
                    };

                    result.Add(teamStats);
                }
            }
            int i = 1;
            foreach(var team in result.OrderByDescending(x=>x.Rating))
            {
                team.Place = i;
                i++;
            }

            return result.OrderByDescending(x => x.Rating).ToList() ;
        }

        public async Task CreateTransferMarket(int userId, List<Position> positions, int budget)
        {
            var state = await GetTeamsState(userId);
            if (state.Team != null)
            {
                if (state.IsCaptain || state.IsAssistant)
                {
                    var team = await _dbContext.Teams.FirstOrDefaultAsync(x => x.Id == state.Team.Id);

                    if (team != null)
                    {
                        _dbContext.TransferMarkets.Add(new TransferMarket
                        {
                            Positions = positions,
                            Team = team,
                            Budget = budget
                        });

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<List<TransferMarketViewModel>> GetTransferMarket()
        {
            var dateTenDaysBefore = DateTime.UtcNow.AddDays(-10);

            var currentSeason = await _seasonService.GetCurrentSeason();

            var result = await _dbContext.TransferMarkets.Include(x=>x.TransferMarketResponses).ThenInclude(x=>x.Player).Include(x => x.Team).ThenInclude(x => x.Season).Where(x => x.CreatedOn > dateTenDaysBefore).Where(x => x.Team.Season == currentSeason).Select(x => new TransferMarketViewModel
            {
                Id = x.Id,
                Date = x.CreatedOn,
                Positions = x.Positions,
                TeamId = x.Team.Id,
                TeamName = x.Team.Name,
                Budget = x.Budget,
                AskedToJoin = x.TransferMarketResponses.Select(p => new TransferMarketAsksViewModel
                {
                    Id = p.Player.Id,
                    Name = p.Player.Name,
                    Positions = p.Position,
                    Cost = p.Player.Cost.Cost
                }).ToList()
            }).ToListAsync();

            return result;
        }

        public async Task RemoveTransferMarket(RemoveTransferMarketRequest request)
        {
            var transferMarket = await _dbContext.TransferMarkets.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (transferMarket != null)
            {
                _dbContext.TransferMarkets.Remove(transferMarket);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task AskToJoinTeam(int userId, Guid transferMarketId, List<Position> positions)
        {
            var state = await GetTeamsState(userId);
            if (state.Team == null)
            {
                var player = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == userId);

                var transferMarket = await _dbContext.TransferMarkets.FirstOrDefaultAsync(x => x.Id == transferMarketId);
                if (transferMarket != null)
                {
                    _dbContext.TransferMarketResponses.Add(new TransferMarketResponse
                    {
                        Player = player,
                        Position = positions,
                        TransferMarket = transferMarket
                    });
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
