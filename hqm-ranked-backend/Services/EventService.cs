using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class EventService: IEventService
    {
        private RankedDb _dbContext;
        public EventService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CurrentEventViewModel> GetCurrentEvent()
        {
            var result = new CurrentEventViewModel();

            var date = DateTime.UtcNow;

            var ev = await _dbContext.Events.Include(x => x.EventType).FirstOrDefaultAsync(x => x.Date.Date == date.Date);

            if (ev == null)
            {
                await CreateNewEvent();
                ev = await _dbContext.Events.Include(x => x.EventType).FirstOrDefaultAsync(x => x.Date.Date == date.Date);
            }

            result.Text = String.Format(ev.EventType.Text, ev.X, ev.Y);
            result.Value = ev.X;
            result.Id = ev.Id;
            result.Left = String.Format("{0}h {1}m", ev.Date.AddDays(1).Subtract(date).Hours, ev.Date.AddDays(1).Subtract(date).Minutes);

            result.Players = await _dbContext.EventWinners.Include(x => x.Event).Include(x => x.Player).Where(x => x.Event == ev).Select(x => new CurrentEventPlayersViewModel
            {
                Id = x.Player.Id,
                Name = x.Player.Name,
                CurrentValue = 0
            }).ToListAsync();

            return result;
        }

        public async Task CreateNewEvent()
        {
            var date = DateTime.UtcNow;

            var ev = await _dbContext.Events.Include(x => x.EventType).FirstOrDefaultAsync(x => x.Date.Date == date.Date);
            if (ev == null)
            {
                var randomEvent = _dbContext.EventTypes.ToList().OrderBy(r => Guid.NewGuid()).FirstOrDefault();
                var x = new Random().Next(randomEvent.MinX, randomEvent.MaxX);
                var y = new Random().Next(randomEvent.MinY, randomEvent.MaxY);
                _dbContext.Events.Add(new Events
                {
                    Date = date.Date,
                    EventType = randomEvent,
                    X = x,
                    Y = y,
                });

                await _dbContext.SaveChangesAsync();
            }
        }


        public async Task CalculateEvents()
        {
            var now = DateTime.UtcNow.Date;

            var ev = await _dbContext.Events.Include(x => x.EventType).FirstOrDefaultAsync(x => x.Date.Date == now.Date);
            if (ev != null)
            {
                var passedUsers = _dbContext.EventWinners.Include(x=>x.Event).Include(x => x.Player).Where(x => x.Event == ev).Select(x => x.Player.Id).ToList();
                var todayGames = _dbContext.Games.Include(x=>x.GamePlayers).ThenInclude(x=>x.Player).Where(x => x.CreatedOn > now).ToList();
                var gamePlayers = todayGames.SelectMany(x => x.GamePlayers).ToList();
                var todayPlayers = gamePlayers.Select(y => y.Player).Distinct().ToList();

                foreach (var player in todayPlayers)
                {
                    var playerGames = gamePlayers.Where(x => x.Player == player).ToList();
                    var passed = false;

                    switch (ev.EventType.Text)
                    {
                        case "Score {0} goals":
                            var goals = gamePlayers.Where(x => x.Player == player).Sum(x => x.Goals);
                            if (goals >= ev.X)
                            {
                                passed = true;
                            }
                            break;
                        case "Do {0} assists":
                            var assists = gamePlayers.Where(x => x.Player == player).Sum(x => x.Assists);
                            if (assists >= ev.X)
                            {
                                passed = true;
                            }
                            break;
                        case "Win {0} games":
                            var countWin = 0;

                            foreach (var game in playerGames)
                            {
                                if (game.Team == 0)
                                {
                                    if (game.Game.RedScore > game.Game.BlueScore)
                                    {
                                        countWin += 1;
                                    }
                                }
                                else
                                {
                                    if (game.Game.RedScore < game.Game.BlueScore)
                                    {
                                        countWin += 1;
                                    }
                                }
                            }

                            if (countWin >= ev.X)
                            {
                                passed = true;
                            }
                            break;
                        case "Win {0} games in a row":
                            var countWinstreak = 0;

                            foreach (var game in playerGames)
                            {
                                if (game.Team == 0)
                                {
                                    if (game.Game.RedScore > game.Game.BlueScore)
                                    {
                                        countWinstreak += 1;
                                    }
                                    else
                                    {
                                        countWinstreak = 0;
                                    }
                                }
                                else
                                {
                                    if (game.Game.RedScore < game.Game.BlueScore)
                                    {
                                        countWinstreak += 1;
                                    }
                                    else
                                    {
                                        countWinstreak = 0;
                                    }
                                }

                                if (countWinstreak >= ev.X)
                                {
                                    passed = true;
                                    break;
                                }
                            }

                            break;
                        case "Do same count of goals and assists more than 0 {0} games":
                            var countSame = 0;

                            foreach (var gameStat in playerGames)
                            {
                                if (gameStat.Goals != 0 && gameStat.Assists != 0)
                                {
                                    if (gameStat.Goals == gameStat.Assists)
                                    {
                                        countSame += 1;
                                    }
                                }
                            }

                            if (countSame >= ev.X)
                            {
                                passed = true;
                            }

                            break;
                    }

                    if (passed)
                    {
                        _dbContext.EventWinners.Add(new EventWinners
                        {
                            Event = ev,
                            Player = player,
                        });
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
