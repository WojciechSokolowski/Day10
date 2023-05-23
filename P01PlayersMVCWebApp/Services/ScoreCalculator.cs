using P01PlayersMVCWebApp.Models;

namespace P01PlayersMVCWebApp.Services
{
    public class ScoreCalculator
    {

        public double CalculateScore(VolleyballPlayer player)
        {
            if (player.MatchesPlayed == 0)
            {
                return 0;
            }
            double score = 5 * (double)player.PointsScored / player.MatchesPlayed + 100 * player.MedalsWon;

            return score;
        }



    }
}
