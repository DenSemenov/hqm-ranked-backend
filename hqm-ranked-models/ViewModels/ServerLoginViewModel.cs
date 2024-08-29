using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class ServerLoginViewModel
    {
        public int Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = String.Empty;
        public string OldNickname { get; set; } = String.Empty;
        public bool SendToAll { get; set; } = false;
        public int Team { get; set; } = 0;
        public LimitsType LimitType { get; set; }
        public double LimitTypeValue { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
