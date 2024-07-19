using hqm_ranked_backend.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class PlayerLiteDataViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gp { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public PlayerCalcStatsViewModel CalcStats { get; set; }
    }
}
