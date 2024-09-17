using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_helpers
{
    public static class PlayoffHelper
    {
        static int[] teamsCount = [2, 4, 8, 16, 32];

        public static List<Game> GenerateBracket(Guid[] teams)
        {
            var games = new List<Game>();

            var minTeamsForFirstRound = teamsCount.Where(x => x <= teams.Length).Max();

            var round = 1;
            for (int i = 0; i < teams.Length - minTeamsForFirstRound; i++)
            {
                var currentRoundTeams = games.Where(x => x.Round == round).Select(x => x.TeamRed).ToList();
                var blueTeams = games.Where(x => x.Round == round).Select(x => x.TeamBlue).ToList();
                currentRoundTeams.AddRange(blueTeams);

                var matchTeams = teams.Where(x => !currentRoundTeams.Contains(x)).Take(2).ToList();
                var teamRed = matchTeams.FirstOrDefault();
                var teamBlue = matchTeams.LastOrDefault();

                games.Add(new Game
                {
                    Round = round,
                    TeamRed = teamRed,
                    TeamBlue = teamBlue,
                    Winner = teamRed,
                });
            }

            var winnerFound = false;
            while (!winnerFound)
            {
                round += 1;

                var currentRoundTeams = games.Where(x => x.Round == round - 1).Select(x => x.Winner).ToList();
                var didnPlayedTeams = teams.Where(x => !games.Any(y => y.TeamRed == x || y.TeamBlue == x)).ToList();
                currentRoundTeams.AddRange(didnPlayedTeams);

                for (int i = 0; i < currentRoundTeams.Count; i += 2)
                {
                    games.Add(new Game
                    {
                        TeamRed = currentRoundTeams[i],
                        TeamBlue = currentRoundTeams[i + 1],
                        Round = round,
                        Winner = currentRoundTeams[i],
                    });
                }

                if (currentRoundTeams.Count == 2)
                {
                    winnerFound = true;
                }
            }

            for (int i = round - 1; i > 0; i--)
            {
                var gamesInRound = games.Where(x => x.Round == i).ToList();
                var nextRoundGames = games.Where(x => x.Round == i + 1).ToList();
                foreach (var gameInRound in gamesInRound)
                {
                    var nextGame = nextRoundGames.FirstOrDefault(x => x.TeamRed == gameInRound.TeamRed || x.TeamBlue == gameInRound.TeamBlue || x.TeamRed == gameInRound.TeamBlue || x.TeamBlue == gameInRound.TeamRed);
                    if (nextGame != null)
                    {
                        gameInRound.NextGameId = nextGame.Id;
                    }
                }
            }

            return games;
        }

        public class Game
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid? NextGameId { get; set; }
            public Guid TeamRed { get; set; }
            public Guid TeamBlue { get; set; }
            public Guid Winner { get; set; }
            public int Round { get; set; }
        }
    }
}
