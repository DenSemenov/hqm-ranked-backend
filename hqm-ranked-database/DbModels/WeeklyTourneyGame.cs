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
        public Game? Game { get; set; }
        public WeeklyTourneyTeam? RedTeam { get; set; }
        public Guid? RedTeamId { get; set; }
        public WeeklyTourneyTeam? BlueTeam { get; set; }
        public Guid? BlueTeamId { get; set; }
        public int PlayoffType { get; set; }
        public WeeklyTourney WeeklyTourney { get; set; }
        public int Index { get; set; } = 0;
        public WeeklyTourneyGame? NextGame { get; set; }
        public Guid? NextGameId { get; set; }
        public Server Server { get; set; }
        public Guid ServerId { get; set; }
    }
}
