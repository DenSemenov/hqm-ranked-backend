namespace hqm_ranked_backend.Models.DTO
{
    public class CalculatedCostStats
    {
        public int Id { get; set; }
        public double Winrate { get; set; }
        public double PointsPerGame { get; set; }
        public int Cost { get; set; }
    }
}
