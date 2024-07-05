namespace hqm_ranked_backend.Models.InputModels
{
    public class ServerLoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
        public string ServerToken { get; set; }
    }
}
