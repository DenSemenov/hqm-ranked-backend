using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class HomeStatsViewModel
    {
        public List<HomeStatsDailyViewModel> Daily { get; set; } = new List<HomeStatsDailyViewModel>();
        public List<HomeStatsWeeklyViewModel> Weekly { get; set; } = new List<HomeStatsWeeklyViewModel>();
    }

    public class HomeStatsDailyViewModel
    {
        public int Hour { get; set; }
        public double Count { get; set; }
    }

    public class HomeStatsWeeklyViewModel
    {
        public string Day { get; set; }
        public double Count { get; set; }
    }
}
