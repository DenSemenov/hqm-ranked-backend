using hqm_ranked_backend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class WeeklyTourneyTeam : AuditableEntity<Guid>
    {
        public WeeklyTourney WeeklyTourney { get; set; }
        public string Name { get; set; }
        public ICollection<WeeklyTourneyPlayer> WeeklyTourneyPlayers { get; set; }
    }
}
