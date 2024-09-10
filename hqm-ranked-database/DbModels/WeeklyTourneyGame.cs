using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class WeeklyTourneyGame : AuditableEntity<Guid>
    {
        public Game Game { get; set; }
        public WeeklyTourneyTeam RedTeam { get; set; }
        public WeeklyTourneyTeam BlueTeam { get; set; }
        public int PlayoffType { get; set; }
        public WeeklyTourney WeeklyTourney { get; set; }
        public int Index { get; set; } = 0;
    }
}
