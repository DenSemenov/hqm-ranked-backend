namespace hqm_ranked_backend.Models.InputModels
{
    public class PickRequest
    {
        public string Token { get; set; }
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public int Team { get; set; }
    }
}
