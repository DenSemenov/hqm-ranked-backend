using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_models.DTO;
using hqm_ranked_models.ViewModels;
using hqm_ranked_services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static SpotifyAPI.Web.PlayerSetRepeatRequest;

namespace hqm_ranked_services
{
    public class WeeklyTourneyService: IWeeklyTourneyService
    {
        private RankedDb _dbContext;
        private ISeasonService _seasonService;
        private IImageGeneratorService _imageGeneratorService;
        private IStorageService _storageService;
        public WeeklyTourneyService(RankedDb dbContext, ISeasonService seasonService, IImageGeneratorService imageGeneratorService, IStorageService storageService)
        {
            _dbContext = dbContext;
            _seasonService = seasonService;
            _imageGeneratorService = imageGeneratorService;
            _storageService = storageService;
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

        public string GetRandomTeamName()
        {
            var adjectives = new List<string>()
            {
                "Awesome", "Brilliant", "Dynamic", "Epic", "Fantastic",
                "Incredible", "Legendary", "Marvelous", "Phenomenal", "Stellar"
            };

            var nouns = new List<string>()
            {
                "Avengers", "Crusaders", "Dynamos", "Eagles", "Guardians",
                "Heroes", "Legends", "Ninjas", "Rangers", "Titans"
            };

            var random = new Random();
            var randomAdjective = adjectives[random.Next(adjectives.Count)];
            var randomNoun = nouns[random.Next(nouns.Count)];

            var teamName = $"{randomAdjective} {randomNoun}";

            return teamName;
        }

        public async Task RandomizeTourney()
        {
            var weekNumber = GetCurrentWeek();
            var tourney = await _dbContext.WeeklyTourneys.Include(x=>x.WeeklyTourneyRequests).ThenInclude(x=>x.Player).ThenInclude(x=>x.Cost).FirstOrDefaultAsync(x => x.WeekNumber == weekNumber);

            if (tourney != null)
            {

                var numTeams = tourney.WeeklyTourneyRequests.Count / 4;

                if (numTeams >= 2)
                {
                    tourney.State = WeeklyTourneyState.Running;

                    var players = tourney.WeeklyTourneyRequests.OrderBy(x => x.CreatedOn).Take(numTeams * 4).Select(x => new
                    {
                        Id = x.Player.Id,
                        Cost = x.Player.Cost != null ? x.Player.Cost.Cost : 100000,

                    }).OrderByDescending(x => x.Cost).ToList();

                    var teams = new List<WeeklyTourneyTeamDTO>();
                    for (int i = 0; i < numTeams; i++)
                    {
                        teams.Add(new WeeklyTourneyTeamDTO());
                    }

                    foreach (var player in players)
                    {
                        foreach (var teamWithMinRating in teams.OrderBy(t => t.TotalRating))
                        {
                            if (teamWithMinRating.Players.Count != 4)
                            {
                                teamWithMinRating.Players.Add(player.Id);
                                teamWithMinRating.TotalRating += player.Cost;
                                break;
                            }
                        }
                    }

                    var newTeams = new List<WeeklyTourneyTeam>();

                    foreach (var team in teams)
                    {
                        var newTeam = new WeeklyTourneyTeam
                        {
                            WeeklyTourney = tourney,
                            Name = GetRandomTeamName(),
                            WeeklyTourneyPlayers = team.Players.Select(x => new WeeklyTourneyPlayer
                            {
                                PlayerId = x
                            }).ToList()
                        };
                        newTeams.Add(newTeam);
                        var teamEntity = _dbContext.WeeklyTourneyTeams.Add(newTeam);

                        var path = String.Format("images/{0}.png", teamEntity.Entity.Id);
                        var file = _imageGeneratorService.GenerateImage();
                        var strm = new MemoryStream();
                        file.SaveAsPng(strm);

                        await _storageService.UploadFileStream(path, strm);
                    }

                    _dbContext.WeeklyTourneyTeams.AddRange(newTeams);

                    for (int i = 0; i < numTeams / 2; i++)
                    {
                        var game = new Game
                        {
                            InstanceType = hqm_ranked_backend.Common.InstanceType.WeeklyTourney,
                            GamePlayers = new List<GamePlayer>(),
                            Season = await _seasonService.GetCurrentSeason(),
                            State = await _dbContext.States.FirstOrDefaultAsync(x => x.Name == "Scheduled")
                        };

                        game.MvpId = newTeams[i].WeeklyTourneyPlayers.FirstOrDefault().PlayerId;

                        _dbContext.WeeklyTourneyGame.Add(new WeeklyTourneyGame
                        {
                            RedTeam = newTeams[i],
                            BlueTeam = newTeams[newTeams.Count - 1 - i],
                            PlayoffType = 0,
                            Game = game,
                            WeeklyTourney = tourney,
                            Index = i
                        });
                    }
                }
                else
                {
                    tourney.State = WeeklyTourneyState.Canceled;
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateTourney()
        {
            var weekNumber = GetCurrentWeek();

            var name = String.Format("Weekly tourney {0}/{1}", weekNumber, DateTime.Now.Year);

            if (!_dbContext.WeeklyTourneys.Any(x => x.Name == name))
            {
                _dbContext.WeeklyTourneys.Add(new hqm_ranked_database.DbModels.WeeklyTourney
                {
                    Name = name,
                    WeekNumber = weekNumber,
                    State = WeeklyTourneyState.Registration,
                    Year = DateTime.Now.Year,
                   
                });

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<WeeklyTourneyViewModel> GetCurrentWeeklyTournament()
        {
            var result = new WeeklyTourneyViewModel();

            var weekNumber = GetCurrentWeek();
            var tr = _dbContext.WeeklyTourneys.FirstOrDefault(x => x.WeekNumber == weekNumber);
           
            if (tr != null && (tr.State == WeeklyTourneyState.Registration || tr.State == WeeklyTourneyState.Running))
            {
                result.State = tr.State;
                if (tr.State == WeeklyTourneyState.Registration)
                {

                    result.Registration = _dbContext.WeeklyTourneys.Include(x => x.WeeklyTourneyRequests).ThenInclude(x => x.Player).Select(x => new WeeklyTourneyRegistrationViewModel
                    {
                        TourneyId = x.Id,
                        TourneyName = x.Name,
                        WeekNumber = x.WeekNumber,
                        Players = x.WeeklyTourneyRequests.OrderByDescending(x => x.CreatedOn).Select(y => new WeeklyTourneyRegistrationPlayerViewModel
                        {
                            Id = y.Player.Id,
                            Name = y.Player.Name
                        }).ToList()
                    }).FirstOrDefault(x => x.WeekNumber == weekNumber);
                }
                else if (tr.State == WeeklyTourneyState.Running)
                {
                    var tourney = _dbContext.WeeklyTourneys
                        .Include(x => x.WeeklyTourneyTeams)
                        .Include(x => x.WeeklyTourneyGames)
                        .ThenInclude(x => x.RedTeam)
                        .Include(x => x.WeeklyTourneyGames)
                        .ThenInclude(x => x.BlueTeam)
                        .Include(x => x.WeeklyTourneyRequests)
                        .ThenInclude(x => x.Player)
                        .Select(x => new
                        {
                            TourneyId = x.Id,
                            TourneyName = x.Name,
                            WeekNumber = x.WeekNumber,
                            Teams = x.WeeklyTourneyTeams,
                            Games = x.WeeklyTourneyGames
                        }).FirstOrDefault(x => x.WeekNumber == weekNumber);

                    result.Tourney = new WeeklyTourneyTourneyViewModel
                    {
                        TourneyId = tourney.TourneyId,
                        TourneyName = tourney.TourneyName,
                        WeekNumber = tourney.WeekNumber,
                        Rounds = tourney.Teams.Count / 2,
                        Games = tourney.Games.Select(x => new WeeklyTourneyGameViewModel
                        {
                            Id = x.Id,
                            RedTeamId = x.RedTeam.Id,
                            BlueTeamId = x.BlueTeam.Id,
                            RedTeamName = x.RedTeam.Name,
                            BlueTeamName = x.BlueTeam.Name,
                            PlayoffType = x.PlayoffType
                        }).ToList()
                    };
                }
            } else
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
            var weeklyTourney = await _dbContext.WeeklyTourneys.FirstOrDefaultAsync(x => x.WeekNumber == weekNumber);
            if (weeklyTourney != null)
            {
                var weeklyTourneyRequest = await _dbContext.WeeklyTourneyRequests.Include(x => x.Player).FirstOrDefaultAsync(x => x.Player.Id == userId);
                if (weeklyTourneyRequest != null )
                {
                    _dbContext.WeeklyTourneyRequests.Remove(weeklyTourneyRequest);
                }
                else
                {
                    _dbContext.WeeklyTourneyRequests.Add(new WeeklyTourneyRequest
                    {
                        PlayerId = userId,
                        WeeklyTourneyId = weeklyTourney.Id,
                        Positions = new List<Position>()
                    });
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
