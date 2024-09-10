using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class WeeklyTourneyRequest : AuditableEntity<Guid>
    {
        public WeeklyTourney WeeklyTourney { get; set; }
        public Guid WeeklyTourneyId { get; set; }
        public Player Player { get; set; }
        public int PlayerId { get; set; }
        public List<Position> Positions { get; set; }
    }
}
