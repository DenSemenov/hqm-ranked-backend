namespace hqm_ranked_backend.Models.InputModels
{
    public class SaveGameRequest
    {
        public string Token { get; set; }
        public Guid GameId { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
    }
}
