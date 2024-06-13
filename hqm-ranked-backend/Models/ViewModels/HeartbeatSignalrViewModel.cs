using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class HeartbeatSignalrViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int LoggedIn { get; set; }
        public int TeamMax { get; set; }
        public int Period { get; set; }
        public int Time { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public int State { get; set; }
        public InstanceType InstanceType { get; set; }
    }
}
