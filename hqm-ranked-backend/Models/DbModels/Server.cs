using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class Server : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public string Token { get; set; }
        public int TeamMax { get; set; }
        public int LoggedIn { get; set; }
        public int Period { get; set; }
        public int Time { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public int State { get; set; }
        public InstanceType InstanceType { get; set; } = InstanceType.Ranked;
    }
}
