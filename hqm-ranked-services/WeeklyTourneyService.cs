﻿using Hangfire;
using hqm_ranked_backend.Common;
using hqm_ranked_backend.Hubs;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_helpers;
using hqm_ranked_models.DTO;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using hqm_ranked_services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using SixLabors.ImageSharp;
using System.Globalization;
using System.Net;

namespace hqm_ranked_services
{
    public class WeeklyTourneyService: IWeeklyTourneyService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        private IImageGeneratorService _imageGeneratorService;
        private IStorageService _storageService;
        private INotificationService _notificationService;
        private readonly IHubContext<ActionHub> _hubContext;
        public WeeklyTourneyService(RankedDb dbContext, ISeasonService seasonService, IImageGeneratorService imageGeneratorService, IStorageService storageService, IHubContext<ActionHub> hubContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
            _imageGeneratorService = imageGeneratorService;
            _storageService = storageService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public int GetCurrentWeek()
        {
            var myCI = new CultureInfo("en-US");
            var myCal = myCI.Calendar;
            var myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            var myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;
            var weekNumber = myCal.GetWeekOfYear(DateTime.UtcNow, myCWR, myFirstDOW);
            return weekNumber;
        }

        public async Task<Guid?> GetCurrentTourneyId()
        {
            var weekNumber = GetCurrentWeek();
            var tourney = await _dbContext.WeeklyTourneys
                .Include(x => x.WeeklyTourneyRequests)
                .ThenInclude(x => x.Player)
                .ThenInclude(x => x.Cost)
                .FirstOrDefaultAsync(x => x.WeekNumber == weekNumber && (x.State ==  WeeklyTourneyState.Registration || x.State == WeeklyTourneyState.Running));

            return tourney !=null? tourney.Id: null;
        }

        public async Task<Guid?> GetCurrentRunningTourneyId()
        {
            var weekNumber = GetCurrentWeek();
            var tourney = await _dbContext.WeeklyTourneys
                .Include(x => x.WeeklyTourneyRequests)
                .ThenInclude(x => x.Player)
                .ThenInclude(x => x.Cost)
                .FirstOrDefaultAsync(x => x.WeekNumber == weekNumber && ( x.State == WeeklyTourneyState.Running));

            return tourney != null ? tourney.Id : null;
        }

        public async Task<List<WeeklyTourneyItemViewModel>> GetWeeklyTourneys()
        {
            var result = await _dbContext.WeeklyTourneys
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new WeeklyTourneyItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    State = x.State
                }).ToListAsync();

            return result;
        }

        public async Task RandomizeTourneyNextStage(int stage)
        {
            var currentTourneyId = await GetCurrentTourneyId();
            var tourney = await _dbContext.WeeklyTourneys.Include(x=>x.WeeklyTourneyGames).ThenInclude(x=>x.Game).Include(x => x.WeeklyTourneyGames).ThenInclude(x=>x.RedTeam).Include(x => x.WeeklyTourneyGames).ThenInclude(x => x.BlueTeam).FirstOrDefaultAsync(x => x.Id == currentTourneyId);

            if (tourney != null)
            {

                var storage = await _seasonService.GetStorage();
                var matchesDto = new List<TourneyMatchesDTO>();
                var games = tourney.WeeklyTourneyGames.Where(x => x.PlayoffType == stage);
                foreach(var game in games)
                {
                    if (game.RedTeamId == null)
                    {
                        var prevGame = tourney.WeeklyTourneyGames.FirstOrDefault(x=>x.NextGameId == game.Id);
                        if (prevGame != null)
                        {
                            if (prevGame.Game != null)
                            {
                                var winnerRed = prevGame.Game.RedScore > prevGame.Game.BlueScore;
                                game.RedTeamId = winnerRed ? prevGame.RedTeamId : prevGame.BlueTeamId;
                            }
                            else
                            {
                                var teamIds = new Guid?[2] { prevGame.RedTeamId, prevGame.BlueTeamId };
                                game.RedTeamId = teamIds.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                            }
                        }
                    }

                    if (game.BlueTeamId == null)
                    {
                        var prevGame = tourney.WeeklyTourneyGames.LastOrDefault(x => x.NextGameId == game.Id);
                        if (prevGame != null)
                        {
                            if (prevGame.Game != null)
                            {
                                var winnerRed = prevGame.Game.RedScore > prevGame.Game.BlueScore;
                                game.BlueTeamId = winnerRed ? prevGame.RedTeamId : prevGame.BlueTeamId;
                            }
                            else
                            {
                                var teamIds = new Guid?[2] { prevGame.RedTeamId, prevGame.BlueTeamId };
                                game.BlueTeamId = teamIds.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                            }
                        }
                    }

                    var redTeam = _dbContext.WeeklyTourneyTeams.FirstOrDefault(x => x.Id == game.RedTeamId);
                    var blueTeam = _dbContext.WeeklyTourneyTeams.FirstOrDefault(x => x.Id == game.BlueTeamId);

                    matchesDto.Add(new TourneyMatchesDTO
                    {
                        RedName = redTeam.Name,
                        BlueName = blueTeam.Name,
                        RedUrl = String.Format(storage + "images/{0}.png", redTeam.Id),
                        BlueUrl = String.Format(storage + "images/{0}.png", blueTeam.Id),
                    });
                }

                tourney.Round = stage;

                if (games.Count() == 0)
                {
                    tourney.State = WeeklyTourneyState.Finished;
                }

                await _dbContext.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("onWeeklyTourneyChange", tourney.Id);

                var lastRound = tourney.WeeklyTourneyGames.Max(x => x.PlayoffType);
                if (tourney.Round < lastRound + 1)
                {
                    BackgroundJob.Schedule(() => this.RandomizeTourneyNextStage(tourney.Round + 1), TimeSpan.FromMinutes(30));
                }

                if (tourney.State != WeeklyTourneyState.Finished)
                {
                    var roundName = GetRoundName(tourney.WeeklyTourneyGames.Max(x => x.PlayoffType), stage);

                    var image = _imageGeneratorService.GenerateMatches(matchesDto, tourney.Name, roundName);
                    var str = new MemoryStream();
                    image.SaveAsPng(str);
                    var newPath = String.Format("images/{0}.png", Guid.NewGuid());
                    await _storageService.UploadFileStream(newPath, str);
                    await _notificationService.SendDiscordTourneyGames(tourney.Name, tourney.Id, storage + newPath);
                }
            }
        }

        public Stream ImageUrlToStream(string imageUrl)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(imageUrl);

                MemoryStream ms = new MemoryStream(imageBytes);

                return ms;
            }
        }

        public string GetRoundName(int maxRound, int current)
        {
            var items = new string[] { "1/32", "1/16", "1/8", "1/4", "Semifinal", "Final" };

            var lastItems = items.TakeLast(maxRound).ToArray();

            return lastItems.ToList()[current - 1];
        }

        public async Task RandomizeTourney()
        {
            var currentTourneyId = await GetCurrentTourneyId();

            var waitingUsers = _dbContext.WeeklyTourneyPartyPlayers.Where(x => x.State == WeeklyTourneyPartyPlayerState.Waiting).ToList();
            for (var i = 0; i < waitingUsers.Count; i++)
            {
                _dbContext.WeeklyTourneyPartyPlayers.Remove(waitingUsers[i]);
            }

            await _dbContext.SaveChangesAsync();    

            var tourney = await _dbContext.WeeklyTourneys.Include(x => x.WeeklyTourneyParties).ThenInclude(x => x.WeeklyTourneyPartyPlayers).ThenInclude(x => x.Player).ThenInclude(x => x.Cost).FirstOrDefaultAsync(x => x.Id == currentTourneyId);

            if (tourney != null)
            {


                var notify = new List<TourneyStartedDTO>();

                var numTeams = tourney.WeeklyTourneyParties.SelectMany(x=>x.WeeklyTourneyPartyPlayers.Where(y=>y.State != WeeklyTourneyPartyPlayerState.Waiting)).Count() / 4;

                if (numTeams >= 4)
                {
                    tourney.State = WeeklyTourneyState.Running;

                    var teams = new List<WeeklyTourneyTeamDTO>();
                    for (int i = 0; i < numTeams; i++)
                    {
                        teams.Add(new WeeklyTourneyTeamDTO());
                    }

                    foreach (var party in tourney.WeeklyTourneyParties.OrderByDescending(x => x.WeeklyTourneyPartyPlayers.Count).ThenByDescending(x => x.WeeklyTourneyPartyPlayers.Sum(y => y.Player.Cost != null ? y.Player.Cost.Cost : 100000)))
                    {
                        var teamWithLowerRating = teams.Where(x => 4 - x.Players.Count >= party.WeeklyTourneyPartyPlayers.Count).OrderBy(t => t.TotalRating).FirstOrDefault();

                        if (teamWithLowerRating != null)
                        {
                            foreach (var player in party.WeeklyTourneyPartyPlayers)
                            {
                                teamWithLowerRating.Players.Add(player.Player.Id);
                                teamWithLowerRating.TotalRating += (player.Player.Cost !=null? player.Player.Cost.Cost: 100000);
                            }
                        }
                        else
                        {
                            foreach (var player in party.WeeklyTourneyPartyPlayers)
                            {
                                var teamWithLowerRatingTemp = teams.OrderBy(t => t.TotalRating).FirstOrDefault();

                                if (teamWithLowerRatingTemp != null)
                                {
                                    if (teamWithLowerRatingTemp.Players.Count < 4)
                                    {
                                        teamWithLowerRatingTemp.Players.Add(player.Player.Id);
                                        teamWithLowerRatingTemp.TotalRating += (player.Player.Cost != null ? player.Player.Cost.Cost : 100000);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Player removed {0} {1}", player.Player.Id, party.Id);
                                    }
                                }
                            }
                        }
                    }

                    var newTeams = new List<WeeklyTourneyTeam>();

                    var nhlTeams = NhlTeamsHelper.GetTeams().OrderBy(x=>Guid.NewGuid()).ToList();

                    var l = 0;
                    var storage = await _seasonService.GetStorage();

                    foreach (var team in teams)
                    {
                        var nhlTeam = nhlTeams[l];
                        var newTeam = new WeeklyTourneyTeam
                        {
                            WeeklyTourney = tourney,
                            Name = nhlTeam.Name,
                            WeeklyTourneyPlayers = team.Players.Select(x => new WeeklyTourneyPlayer
                            {
                                PlayerId = x
                            }).ToList()
                        };
                        newTeams.Add(newTeam);
                        var teamEntity = _dbContext.WeeklyTourneyTeams.Add(newTeam);

                        var path = String.Format("images/{0}.png", teamEntity.Entity.Id);
                        var strm = ImageUrlToStream(nhlTeam.Url);

                        await _storageService.UploadFileStream(path, strm);

                        var discordPlayers = await _dbContext.Players.Where(x => team.Players.Contains(x.Id)).Select(x=>new TourneyStartedPlayerDTO
                        {
                             Name = x.Name,
                              DiscordId = x.DiscordId
                        }).ToListAsync();

                        notify.Add(new TourneyStartedDTO
                        {
                            Name = nhlTeam.Name,
                            AvatarUrl = storage + path,
                            Players = discordPlayers
                        });

                        l += 1;
                    }

                    _dbContext.WeeklyTourneyTeams.AddRange(newTeams);

                    var matches = PlayoffHelper.GenerateBracket(newTeams.Select(x => x.Id).ToArray());

                    var round = 1;
                    var index = 0;

                    var servers = _dbContext.Servers.Where(x => x.InstanceType == InstanceType.WeeklyTourney).ToList();

                    var matchesDto = new List<TourneyMatchesDTO>();

                    foreach(var match in matches.OrderByDescending(x=>x.Round))
                    {
                        if (round != match.Round)
                        {
                            round = match.Round;
                            index = 0;
                        }

                        Guid? teamRed = match.TeamRed;
                        Guid? teamBlue = match.TeamBlue;

                        var matchesPrevRound = matches.Where(x => x.Round == match.Round - 1);

                        if (matchesPrevRound.Any(x => x.TeamRed == teamRed || x.TeamBlue == teamRed)){
                            teamRed = null;
                        }

                        if (matchesPrevRound.Any(x => x.TeamRed == teamBlue || x.TeamBlue == teamBlue))
                        {
                            teamBlue = null;
                        }

                        _dbContext.WeeklyTourneyGame.Add(new WeeklyTourneyGame
                        {
                            Id = match.Id,
                            NextGameId = match.NextGameId,
                            WeeklyTourney = tourney,
                            RedTeamId = teamRed,
                            BlueTeamId = teamBlue,
                            PlayoffType = match.Round,
                            Index = index,
                            Server = servers[index]
                        }) ;

                        if (round == 1)
                        {
                            var redTeam = newTeams.FirstOrDefault(x => x.Id == teamRed);
                            var blueTeam = newTeams.FirstOrDefault(x => x.Id == teamBlue);

                            matchesDto.Add(new TourneyMatchesDTO
                            {
                                RedName = redTeam.Name,
                                BlueName = blueTeam.Name,
                                RedUrl = String.Format(storage + "images/{0}.png", redTeam.Id),
                                BlueUrl = String.Format(storage + "images/{0}.png", blueTeam.Id),
                            });
                        }

                        index += 1;
                    }

                    await _dbContext.SaveChangesAsync();

                    BackgroundJob.Schedule(() => this.RandomizeTourneyNextStage(tourney.Round + 1), TimeSpan.FromMinutes(30));

                    await _notificationService.SendDiscordTourneyStarted(tourney.Name, tourney.Id, notify);

                    var roundName = GetRoundName(matches.Max(x=>x.Round), 1);

                    var image = _imageGeneratorService.GenerateMatches(matchesDto, tourney.Name, roundName);
                    var str = new MemoryStream();
                    image.SaveAsPng(str);
                    var newPath = String.Format("images/{0}.png", Guid.NewGuid());
                    await _storageService.UploadFileStream(newPath, str);
                    await _notificationService.SendDiscordTourneyGames(tourney.Name, tourney.Id, storage + newPath);
                }
                else
                {
                    tourney.State = WeeklyTourneyState.Canceled;

                    await _dbContext.SaveChangesAsync();
                }

                await _hubContext.Clients.All.SendAsync("onWeeklyTourneyChange", tourney.Id);
            }
        }

        public async Task CreateTourney()
        {
            var weekNumber = GetCurrentWeek();

            var name = String.Format("Weekly tourney {0}/{1}", weekNumber, DateTime.Now.Year);

            if (!_dbContext.WeeklyTourneys.Any(x => x.Name == name))
            {
                var entity = _dbContext.WeeklyTourneys.Add(new hqm_ranked_database.DbModels.WeeklyTourney
                {
                    Name = name,
                    WeekNumber = weekNumber,
                    State = WeeklyTourneyState.Registration,
                    Year = DateTime.Now.Year,
                   
                });

                await _dbContext.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("onWeeklyTourneyChange", entity.Entity.Id);

                BackgroundJob.Schedule(() => this.RandomizeTourney(), TimeSpan.FromMinutes(30));

                await _notificationService.SendDiscordRegistrationStarted(name, entity.Entity.Id);
            }
        }

        public async Task<WeeklyTourneyViewModel> GetWeeklyTournament(WeeklyTourneyIdRequest request)
        {
            var result = new WeeklyTourneyViewModel();

            var tr = await  _dbContext.WeeklyTourneys.Include(x => x.WeeklyTourneyRequests).ThenInclude(x => x.Player).ThenInclude(x => x.Cost).FirstOrDefaultAsync(x => x.Id == request.Id);

            if (tr != null && (tr.State == WeeklyTourneyState.Registration || tr.State == WeeklyTourneyState.Running || tr.State == WeeklyTourneyState.Finished))
            {
                result.State = tr.State;
                if (tr.State == WeeklyTourneyState.Registration)
                {

                    result.Registration = _dbContext.WeeklyTourneys.Include(x=>x.WeeklyTourneyParties).ThenInclude(x=>x.WeeklyTourneyPartyPlayers).ThenInclude(x => x.Player).Include(x => x.WeeklyTourneyRequests).ThenInclude(x => x.Player).Select(x => new WeeklyTourneyRegistrationViewModel
                    {
                        TourneyId = x.Id,
                        TourneyName = x.Name,
                        WeekNumber = x.WeekNumber,
                        Parties = x.WeeklyTourneyParties.Select(y => new WeeklyTourneyRegistrationPartyViewModel
                        {
                            PartyId = y.Id,
                            Players = y.WeeklyTourneyPartyPlayers.OrderBy(x=>x.CreatedOn).Select(y => new WeeklyTourneyRegistrationPlayerViewModel
                            {
                                Id = y.Player.Id,
                                Name = y.Player.Name,
                                State = y.State
                            }).ToList()
                        }).ToList(),
                        Players = x.WeeklyTourneyRequests.OrderByDescending(x => x.CreatedOn).Select(y => new WeeklyTourneyRegistrationPlayerViewModel
                        {
                            Id = y.Player.Id,
                            Name = y.Player.Name
                        }).ToList()
                    }).FirstOrDefault(x => x.TourneyId == request.Id);

                    result.Registration.AllPlayers = _dbContext.Players.Include(x=>x.Bans).Where(x=>!x.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow) && x.IsApproved).OrderBy(x => x.Name).Select(x => new WeeklyTourneyRegistrationPlayerViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();
                }
                else if (tr.State == WeeklyTourneyState.Running || tr.State == WeeklyTourneyState.Finished)
                {
                    result.Tourney = _dbContext.WeeklyTourneys
                        .Include(x => x.WeeklyTourneyTeams)
                        .ThenInclude(x => x.WeeklyTourneyPlayers)
                        .ThenInclude(x => x.Player)
                        .Include(x => x.WeeklyTourneyGames)
                        .ThenInclude(x => x.RedTeam)
                        .Include(x => x.WeeklyTourneyGames)
                        .ThenInclude(x => x.BlueTeam)
                        .Include(x => x.WeeklyTourneyGames)
                        .ThenInclude(x => x.Game)
                        .ThenInclude(x => x.GamePlayers)
                         .Include(x => x.WeeklyTourneyGames)
                        .ThenInclude(x => x.Game)
                        .ThenInclude(x => x.State)
                        .Include(x => x.WeeklyTourneyRequests)
                        .ThenInclude(x => x.Player)
                        .Select(tourney => new WeeklyTourneyTourneyViewModel
                        {
                            TourneyId = tourney.Id,
                            TourneyName = tourney.Name,
                            WeekNumber = tourney.WeekNumber,
                            Rounds = (int)Math.Ceiling(((double)tourney.WeeklyTourneyTeams.Count - 1) / 2),
                            Teams = tourney.WeeklyTourneyTeams.Select(x => new WeeklyTourneyTeamViewModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Players = x.WeeklyTourneyPlayers.Select(y => new WeeklyTourneyTeamPlayerViewModel
                                {
                                    Id = y.PlayerId,
                                    Name = y.Player.Name,
                                    Goals = 0,
                                    Assists = 0,
                                    Points = 0
                                }).ToList()
                            }).ToList(),
                            Games = tourney.WeeklyTourneyGames.Select(x => new WeeklyTourneyGameViewModel
                            {
                                Id = x.Id,
                                RedTeamId = x.RedTeamId,
                                BlueTeamId = x.BlueTeamId,
                                RedTeamName = x.RedTeam != null ? x.RedTeam.Name : String.Empty,
                                BlueTeamName = x.BlueTeam != null ? x.BlueTeam.Name : String.Empty,
                                RedScore = x.Game != null ? x.Game.RedScore : 0,
                                BlueScore = x.Game != null ? x.Game.BlueScore : 0,
                                PlayoffType = x.PlayoffType,
                                Index = x.Index,
                                NextGameId = x.NextGameId,
                                State = x.Game != null ? x.Game.State.Name : "Scheduled",
                                GamePlayers = x.Game != null ? x.Game.GamePlayers.Select(y => new WeeklyTourneyGamePlayerViewModel
                                {
                                    PlayerId = y.PlayerId,
                                    Assists = y.Assists,
                                    Goals = y.Goals
                                }).ToList() : new List<WeeklyTourneyGamePlayerViewModel>(),
                            }).OrderBy(x => x.PlayoffType).ThenBy(x => x.Index).ToList()
                        }).FirstOrDefault(x => x.TourneyId == request.Id);

                    var allGamePlayers = result.Tourney.Games.SelectMany(x => x.GamePlayers).ToList();

                    foreach (var team in result.Tourney.Teams)
                    {
                        foreach (var player in team.Players)
                        {

                            player.Gp = allGamePlayers.Count(x => x.PlayerId == player.Id);
                            player.Goals = allGamePlayers.Where(x => x.PlayerId == player.Id).Sum(x => x.Goals);
                            player.Assists = allGamePlayers.Where(x => x.PlayerId == player.Id).Sum(x => x.Assists);
                            player.Points = player.Goals + player.Assists;
                        }
                    }
                }
            }
            else
            {
                var today = DateTime.UtcNow;
                int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
                var nextSaturday = today.AddDays(daysUntilSaturday);

                var saturdayAt530UTC = new DateTime(nextSaturday.Year, nextSaturday.Month, nextSaturday.Day, 17, 30, 0, DateTimeKind.Utc);
                result.StartDate = saturdayAt530UTC;
                result.State = WeeklyTourneyState.Canceled;
            }

            return result;
        }

        public async Task WeeklyTourneyRegister(int userId)
        {
            var weekNumber = GetCurrentWeek();
            var weeklyTourney = await _dbContext.WeeklyTourneys.Include(x => x.WeeklyTourneyParties).ThenInclude(x => x.WeeklyTourneyPartyPlayers).ThenInclude(x => x.Player).FirstOrDefaultAsync(x => x.WeekNumber == weekNumber);
            if (weeklyTourney != null)
            {
                var user = await _dbContext.Players.Include(x => x.Bans).SingleOrDefaultAsync(x => x.Id == userId);
                var isBanned = user.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow);
                if (!isBanned && user.IsApproved)
                {
                    var partiesWaitings = weeklyTourney.WeeklyTourneyParties.Where(x => x.WeeklyTourneyPartyPlayers.Any(x => x.Player.Id == userId && x.State == WeeklyTourneyPartyPlayerState.Waiting)).ToList();
                    foreach(var partiesWaiting in partiesWaitings)
                    {
                        var playerInWaitingParty = partiesWaiting.WeeklyTourneyPartyPlayers.FirstOrDefault(x => x.Player.Id == userId);
                        if (playerInWaitingParty != null)
                        {
                            partiesWaiting.WeeklyTourneyPartyPlayers.Remove(playerInWaitingParty);
                        }
                    }

                    var party = weeklyTourney.WeeklyTourneyParties.FirstOrDefault(x => x.WeeklyTourneyPartyPlayers.Any(x => x.Player.Id == userId));
                    if (party != null)
                    {
                        var playerInParty = party.WeeklyTourneyPartyPlayers.FirstOrDefault(x => x.Player.Id == userId);
                        if (playerInParty != null)
                        {
                            if (playerInParty.State == WeeklyTourneyPartyPlayerState.Host)
                            {
                                weeklyTourney.WeeklyTourneyParties.Remove(party);
                            }
                            else
                            {
                                party.WeeklyTourneyPartyPlayers.Remove(playerInParty);
                            }
                        }
                        
                    }
                    else
                    {
                        weeklyTourney.WeeklyTourneyParties.Add(new WeeklyTourneyParty
                        {
                            WeeklyTourneyPartyPlayers = new List<WeeklyTourneyPartyPlayer>
                             {
                                 new WeeklyTourneyPartyPlayer
                                 {
                                      Player = user,
                                      State = WeeklyTourneyPartyPlayerState.Host
                                 }
                             }
                        });
                    }

                    await _dbContext.SaveChangesAsync();

                    await _hubContext.Clients.All.SendAsync("onWeeklyTourneyChange", weeklyTourney.Id);
                }
            }
        }

        public async Task WeeklyTourneyInvite(int userId, int invitedId)
        {
            var weekNumber = GetCurrentWeek();
            var weeklyTourney = await _dbContext.WeeklyTourneys.Include(x => x.WeeklyTourneyParties).ThenInclude(x => x.WeeklyTourneyPartyPlayers).ThenInclude(x => x.Player).FirstOrDefaultAsync(x => x.WeekNumber == weekNumber);
            if (weeklyTourney != null)
            {
                var user = await _dbContext.Players.Include(x => x.Bans).SingleOrDefaultAsync(x => x.Id == userId);
                var invited = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == invitedId);
                var isBanned = user.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow);
                if (!isBanned)
                {
                    var party = weeklyTourney.WeeklyTourneyParties.FirstOrDefault(x => x.WeeklyTourneyPartyPlayers.Any(x => x.Player.Id == userId && x.State == WeeklyTourneyPartyPlayerState.Host));
                    if (party != null)
                    {
                        var playerInParty = party.WeeklyTourneyPartyPlayers.FirstOrDefault(x => x.Player == invited);
                        if (playerInParty == null)
                        {
                            party.WeeklyTourneyPartyPlayers.Add(new WeeklyTourneyPartyPlayer
                            {
                                Player = invited,
                                State = WeeklyTourneyPartyPlayerState.Waiting
                            });
                        }
                        else
                        {
                            party.WeeklyTourneyPartyPlayers.Remove(playerInParty);
                        }

                        await _dbContext.SaveChangesAsync();
                    }

                    await _hubContext.Clients.All.SendAsync("onWeeklyTourneyChange", weeklyTourney.Id);
                }
            }
        }

        public async Task WeeklyTourneyAcceptDeclineInvite(int userId, WeeklyTourneyAcceptDeclineInvite request)
        {
            var weekNumber = GetCurrentWeek();
            var weeklyTourney = await _dbContext.WeeklyTourneys.Include(x => x.WeeklyTourneyParties).ThenInclude(x => x.WeeklyTourneyPartyPlayers).ThenInclude(x => x.Player).FirstOrDefaultAsync(x => x.WeekNumber == weekNumber);
            if (weeklyTourney != null)
            {
                var user = await _dbContext.Players.Include(x => x.Bans).SingleOrDefaultAsync(x => x.Id == userId);

                var party = weeklyTourney.WeeklyTourneyParties.FirstOrDefault(x => x.Id == request.Id);
                if (party != null)
                {
                    var invited = party.WeeklyTourneyPartyPlayers.FirstOrDefault(x => x.Player == user);
                    if (invited != null)
                    {
                        if (request.IsAccepted)
                        {
                            invited.State = WeeklyTourneyPartyPlayerState.Accepted;
                        }else
                        {
                            party.WeeklyTourneyPartyPlayers.Remove(invited);
                        }

                        await _dbContext.SaveChangesAsync();

                        await _hubContext.Clients.All.SendAsync("onWeeklyTourneyChange", weeklyTourney.Id);
                    }
                }

            }
        }
    }
}
