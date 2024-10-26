using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.InputModels
{
    public class WeeklyTourneyAcceptDeclineInvite
    {
        public Guid Id { get; set; }
        public bool IsAccepted { get; set; }
    }
}
