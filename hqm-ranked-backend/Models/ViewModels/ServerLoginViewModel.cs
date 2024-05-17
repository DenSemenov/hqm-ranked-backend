namespace hqm_ranked_backend.Models.ViewModels
{
    public class ServerLoginViewModel
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = String.Empty;
        public string OldNickname { get; set; } = String.Empty;
        public bool SendToAll { get; set; } = false;
    }
}
