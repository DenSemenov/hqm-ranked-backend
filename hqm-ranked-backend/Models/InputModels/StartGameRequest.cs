namespace hqm_ranked_backend.Models.InputModels
{
    public class StartGameRequest
    {
        public string Token { get; set; }
        public int MaxCount { get; set; }
        public List<Guid> PlayerIds { get; set; }
    }
}
