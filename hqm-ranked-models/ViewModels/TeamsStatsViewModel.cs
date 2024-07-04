using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class TeamsStatsViewModel
    {
        public List<TeamsStatsTeamViewModel> Teams { get; set; } = new List<TeamsStatsTeamViewModel>();
        public List<TeamsStatsPlayerViewModel> Players { get; set; } = new List<TeamsStatsPlayerViewModel>();
    }

    public class TeamsStatsTeamViewModel
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

    public class TeamsStatsPlayerViewModel
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; }
        public int Place { get; set; }
        public int PlayerId { get; set; }
        public string Nickname { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Mvp { get; set; }
        public int Rating { get; set; }
    }
}
