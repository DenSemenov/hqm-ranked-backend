using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class TeamsStatsViewModel
    {
        public int Place { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Goals { get; set; }
        public int GoalsConceded { get; set; }
        public int Rating { get; set; }
    }
}
