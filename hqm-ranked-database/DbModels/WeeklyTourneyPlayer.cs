using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class WeeklyTourneyPlayer : AuditableEntity<Guid>
    {
        public WeeklyTourneyTeam Team { get; set; }
        public Player Player { get; set; }
        public int PlayerId { get; set; }
    }
}
