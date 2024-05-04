namespace hqm_ranked_backend.Models.DTO
{
    public class EloCalcModel
    {
        public int RedScore { get;set; }
        public int BlueScore { get; set; }
        public List<EloCalcPlayerModel> Players { get; set; } = new List<EloCalcPlayerModel>();

    }

    public class EloCalcPlayerModel
    {
        public int Id { get; set; }
        public int Team { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Points { get; set; }
        public int Elo { get; set; }
        public int Performance { get; set; }
        public int RawScore { get; set; }
    }
}
