using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class WeeklyTourneyItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WeeklyTourneyState State { get; set; }
    }
}
