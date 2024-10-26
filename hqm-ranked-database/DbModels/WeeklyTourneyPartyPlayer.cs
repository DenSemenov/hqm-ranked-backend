using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class WeeklyTourneyPartyPlayer : AuditableEntity<Guid>
    {
        public WeeklyTourneyParty WeeklyTourneyParty {  get; set; }
        public Player Player { get; set; }
        public WeeklyTourneyPartyPlayerState State { get; set; }
    }

    public enum WeeklyTourneyPartyPlayerState
    { 
        Host,
        Waiting,
        Accepted
    }
}
