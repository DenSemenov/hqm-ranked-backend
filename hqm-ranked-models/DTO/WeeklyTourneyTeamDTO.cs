using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.DTO
{
    public class WeeklyTourneyTeamDTO
    {
        public List<int> Players { get; set; } = new List<int>();
        public int TotalRating { get; set; }
    }
}
