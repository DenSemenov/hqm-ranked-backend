using hqm_ranked_backend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class Contract : AuditableEntity<Guid>
    {
        public ContractType ContractType { get; set; }
        public int Count { get; set; }
        public int Points { get; set; }
    }

    public enum ContractType
    {
        Assists,
        WinWith800Elo,
        Saves,
        WinWith20Possesion,
        Winstreak,
        RiseInRanking
    }
}
