using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class TransferMarketViewModel
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set;}
        public DateTime Date { get; set; }
        public List<Position> Positions { get; set; }
        public int Budget { get; set; }
        public List<TransferMarketAsksViewModel> AskedToJoin { get; set; } = new List<TransferMarketAsksViewModel>();
    }

    public class TransferMarketAsksViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Position> Positions { get; set; }
        public int Cost { get; set; }
    }
}
