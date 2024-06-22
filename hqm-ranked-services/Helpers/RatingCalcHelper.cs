using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.DTO;

namespace hqm_ranked_backend.Helpers
{
    public static class RatingCalcHelper
    {
        const int RED_TEAM = 0;
        const int BLUE_TEAM = 1;

        const int PERFORMANCE_BONUS = 5;
        public static EloCalcModel CalcRating(EloCalcModel game)
        {
            var totalEloRedTeam = 0;
            var totalEloBlueTeam = 0;
            var totalPointsRedTeam = 0;
            var totalPointsBlueTeam = 0;
            var playerCount = game.Players.Count;

            foreach (var player in game.Players)
            {
                if (player.Team == RED_TEAM)
                {
                    totalEloRedTeam += player.Elo;
                    totalPointsRedTeam += player.Points;
                }
                if (player.Team == BLUE_TEAM)
                {
                    totalEloBlueTeam += player.Elo;
                    totalPointsBlueTeam += player.Points;
                }
            }

            var avgPointsRedTeam = totalPointsRedTeam / (playerCount / 2);
            var avgPointsBlueTeam = totalPointsBlueTeam / (playerCount / 2);

            var winnerTeam = game.RedScore > game.BlueScore ? 0 : 1;

            var redProbFactor = totalEloRedTeam > totalEloBlueTeam ? 1 : -1;

            double blueRedEloDivide = 1;

            if (totalEloRedTeam != 0)
            {
                blueRedEloDivide = (double)totalEloBlueTeam / (double)totalEloRedTeam;
            }

            var winProbRed = Math.Max(Math.Min(0.5 + redProbFactor * Math.Abs(blueRedEloDivide - 1), 0.99), 0.01);
            var winProbBlue = 1 - winProbRed;

            foreach (var player in game.Players)
            {
                if (player.Team != winnerTeam)
                {
                    var avgPointsOtherTeam = player.Team == RED_TEAM ? avgPointsBlueTeam : avgPointsRedTeam;
                    if (player.Points > avgPointsOtherTeam)
                    {
                        player.Performance = PERFORMANCE_BONUS;
                    }
                }

                if (player.Team == RED_TEAM)
                {
                    player.RawScore = (int)(player.Team == winnerTeam ? Math.Round(50 * winProbBlue) : Math.Round(-50 * winProbRed));
                    game.RedPoints = player.RawScore;
                }
                if (player.Team == BLUE_TEAM)
                {
                    player.RawScore = (int)(player.Team == winnerTeam ? Math.Round(50 * winProbRed) : Math.Round(-50 * winProbBlue));
                    game.BluePoints = player.RawScore;
                }

                player.Elo = player.Performance + player.RawScore;
            }

            return game;
        }
    }
}
