using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class WeeklyTourney : AuditableEntity<Guid>
    {
        public string Name { get; set; }
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public int Round { get; set; } = 1;
        public WeeklyTourneyState State { get; set; }
        public ICollection<WeeklyTourneyRequest> WeeklyTourneyRequests { get; set; }
        public ICollection<WeeklyTourneyParty> WeeklyTourneyParties { get; set; }
        public ICollection<WeeklyTourneyGame> WeeklyTourneyGames { get; set; }
        public ICollection<WeeklyTourneyTeam> WeeklyTourneyTeams { get; set; }
    }

    public enum WeeklyTourneyState
    {
        Registration,
        Running,
        Canceled,
        Finished
    }
}
