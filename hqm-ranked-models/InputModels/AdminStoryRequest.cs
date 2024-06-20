namespace hqm_ranked_backend.Models.InputModels
{
    public class AdminStoryRequest
    {
        public string Text { get; set; }
        public bool Expiration { get; set; }
        public string Link { get; set; }
    }
}
