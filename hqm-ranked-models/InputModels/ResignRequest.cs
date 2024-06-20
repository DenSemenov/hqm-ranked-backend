namespace hqm_ranked_backend.Models.InputModels
{
    public class ResignRequest
    {
        public string Token { get; set; }
        public Guid GameId { get; set; }
        public int Team { get; set; }
    }
}
