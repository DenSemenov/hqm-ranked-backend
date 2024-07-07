using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class PlayerCalcStats : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public double Mvp { get; set; }
        public double Winrate { get; set; }
        public double Goals { get; set; }
        public double Assists { get; set; }
        public double Shots { get; set; }
        public double Saves { get; set; }

    }
}
