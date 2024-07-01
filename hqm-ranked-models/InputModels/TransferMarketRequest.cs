using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.InputModels
{
    public class TransferMarketRequest
    {  
        public List<Position> Positions { get; set; }
        public int Budget { get; set; }
    }
}
