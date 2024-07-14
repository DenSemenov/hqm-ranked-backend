using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;

namespace hqm_ranked_database.DbModels
{
    public class PlayerLogin : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public string Ip { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public LoginInstance LoginInstance { get; set; }
        public string UserAgent { get; set; }
        public string AcceptLang { get; set; }
        public string Browser { get; set; }
        public string Platform { get; set; }
    }

    public enum LoginInstance
    {
        Web,
        Server
    }
}
