namespace hqm_ranked_backend.Models.InputModels
{
    public class ResetRequest
    {
        public string Token { get; set; }
        public Guid GameId { get; set; }
    }
}
