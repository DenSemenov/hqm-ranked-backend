using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.DTO
{
    public class PlayerCalcModel
    {
        public int PlayerId { get; set; }
        public double Mvp { get; set; }
        public double Winrate { get; set; }
        public double Gpg { get; set; }
        public double Apg { get; set; }
        public double Shots { get; set; }
        public double Saves { get; set; }
    }
}
