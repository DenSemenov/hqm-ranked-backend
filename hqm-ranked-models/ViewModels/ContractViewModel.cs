using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class ContractViewModel
    {
        public ContractType ContractType { get; set; }
        public int Count { get; set; }
        public int Points { get; set; }
        public bool CanSelect { get; set; }
    }
}
