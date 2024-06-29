using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.DTO
{
    public class SeasonEloModel
    {
        public Guid SeasonId { get; set; }
        public int Elo {  get; set; }
    }
}
