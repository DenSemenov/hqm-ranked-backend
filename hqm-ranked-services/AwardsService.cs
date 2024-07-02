using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using hqm_ranked_database.DbModels;
using hqm_ranked_services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_services
{
    public class AwardsService: IAwardsService
    {
        private RankedDb _dbContext;
        private INotificationService _notificationService;
        private ISeasonService _seasonService;

        private int[] CountsToCheck = [500, 1000, 2500, 5000, 10000];
        public AwardsService(RankedDb dbContext, INotificationService notificationService, ISeasonService seasonService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
            _seasonService = seasonService;
        }

        public async Task CalcAwards()
        {
            var currentAwards = await _dbContext.Awards
                .Include(x => x.Player)
                .Include(x => x.Season)
                .Select(x => new
                {
                    Id = x.PlayerId,
                    AwardType = x.AwardType,
                    Season = x.Season,
                    Count = x.Count
                })
                .ToListAsync();

            var players = await _dbContext.Players
                .Include(x => x.GamePlayers)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    GamesPlayed = x.GamePlayers.Count(),
                    Goals = x.GamePlayers.Sum(x => x.Goals),
                    Assists = x.GamePlayers.Sum(x => x.Assists),
                })
                .ToListAsync();

            var awardsToAdd = new List<Award>();

            foreach (var count in CountsToCheck)
            {
                var playersWithTheseGamesCount = players.Where(x => x.GamesPlayed >= count && !currentAwards.Any(y => y.AwardType == AwardType.GamesPlayed && y.Id == x.Id && y.Count == count));
                var playersWithTheseGoalsCount = players.Where(x => x.Goals >= count && !currentAwards.Any(y => y.AwardType == AwardType.Goals && y.Id == x.Id && y.Count == count));
                var playersWithTheseAssistsCount = players.Where(x => x.Assists >= count && !currentAwards.Any(y => y.AwardType == AwardType.Assists && y.Id == x.Id && y.Count == count));

                foreach (var pl in playersWithTheseGamesCount)
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.GamesPlayed,
                        PlayerId = pl.Id,
                        Count = count,
                    });
                }
                foreach (var pl in playersWithTheseGoalsCount)
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.Goals,
                        PlayerId = pl.Id,
                        Count = count,
                    });
                }
                foreach (var pl in playersWithTheseAssistsCount)
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.Assists,
                        PlayerId = pl.Id,
                        Count = count,
                    });
                }
            }

            var dateNow = DateTime.UtcNow;
            var endedSeasons = await _dbContext.Seasons.Where(x => x.DateEnd < dateNow).OrderBy(x => x.DateStart).ToListAsync();

            foreach (var season in endedSeasons)
            {
                var seasonData = await _seasonService.GetSeasonStats(new hqm_ranked_backend.Models.InputModels.CurrentSeasonStatsRequest
                {
                    Offset = 0,
                    SeasonId = season.Id,
                });

                var firstPlace = seasonData[0];
                var secondPlace = seasonData[1];
                var thirdPlace = seasonData[2];

                var bestGoleador = seasonData.Where(x => x.Win + x.Lose >= 20).OrderByDescending(x => (double)x.Goals / (double)(x.Win = x.Lose) * Math.Clamp(((x.Win = x.Lose) * 0.01), 0, 1)).FirstOrDefault();
                var bestAssistant = seasonData.Where(x => x.Win + x.Lose >= 20 && x.PlayerId != bestGoleador.PlayerId).OrderByDescending(x => (double)x.Assists / (double)(x.Win = x.Lose) * Math.Clamp(((x.Win = x.Lose) * 0.01), 0, 1)).FirstOrDefault();

                if (!currentAwards.Any(y => y.AwardType == AwardType.FirstPlace && y.Id == firstPlace.PlayerId && y.Season == season))
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.FirstPlace,
                        PlayerId = firstPlace.PlayerId,
                        Season = season,
                    });
                }

                if (!currentAwards.Any(y => y.AwardType == AwardType.SecondPlace && y.Id == secondPlace.PlayerId && y.Season == season))
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.SecondPlace,
                        PlayerId = secondPlace.PlayerId,
                        Season = season,
                    });
                }

                if (!currentAwards.Any(y => y.AwardType == AwardType.ThirdPlace && y.Id == thirdPlace.PlayerId && y.Season == season))
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.ThirdPlace,
                        PlayerId = thirdPlace.PlayerId,
                        Season = season,
                    });
                }

                if (!currentAwards.Any(y => y.AwardType == AwardType.BestGoaleador && y.Id == bestGoleador.PlayerId && y.Season == season))
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.BestGoaleador,
                        PlayerId = bestGoleador.PlayerId,
                        Season = season,
                    });
                }

                if (!currentAwards.Any(y => y.AwardType == AwardType.BestAssistant && y.Id == bestAssistant.PlayerId && y.Season == season))
                {
                    awardsToAdd.Add(new Award
                    {
                        AwardType = AwardType.BestAssistant,
                        PlayerId = bestAssistant.PlayerId,
                        Season = season,
                    });
                }
            }

            foreach (var award in awardsToAdd)
            {
                var player = players.FirstOrDefault(x => x.Id == award.PlayerId);
                await _notificationService.SendDiscordNewsAward(award, player.Name);
            }

            _dbContext.Awards.AddRange(awardsToAdd);
            await _dbContext.SaveChangesAsync();
        }
    }
}
