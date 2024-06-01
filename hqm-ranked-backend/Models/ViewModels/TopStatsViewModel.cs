namespace hqm_ranked_backend.Models.ViewModels
{
    public class TopStatsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gp { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Wins { get; set; }
        public int Loses { get; set; }
        public double GoalsPerGame { get; set; }
        public double AssistsPerGame { get; set; }
        public double Winrate { get; set; }
        public int Elo { get; set; }
    }
}
