using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class PlayerMapViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool IsHidden { get; set; }
        public double Lon {  get; set; }
        public double Lat { get; set; }
    }
}
