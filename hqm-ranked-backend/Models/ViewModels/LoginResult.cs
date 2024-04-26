namespace hqm_ranked_backend.Models.ViewModels
{
    public class LoginResult
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public bool Success { get; set; }
        public bool IsExists { get; set; }
    }
}
