using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class PlayerLogin : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public string Ip { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public LoginInstance LoginInstance { get; set; }
    }

    public enum LoginInstance
    {
        Web,
        Server
    }
}
