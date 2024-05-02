namespace hqm_ranked_backend.Models.InputModels
{
    public class HeartbeatRequest
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public int LoggedIn { get; set; }
        public int TeamMax { get; set; }
        public int Period { get; set; }
        public int Time { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public int State { get; set; }
    }
}
