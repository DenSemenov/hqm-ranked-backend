﻿using hqm_ranked_backend.Common;
using hqm_ranked_backend.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_database.DbModels
{
    public class Award : AuditableEntity<Guid>
    {
        public Player Player { get; set; }
        public int PlayerId { get; set; }
        public AwardType AwardType { get; set; }
        public Season? Season { get; set; }
        public int? Count { get; set; }
    }

    public enum AwardType
    {
        FirstPlace,
        SecondPlace,
        ThirdPlace,
        BestGoaleador,
        BestAssistant,
        GamesPlayed,
        Goals,
        Assists
    }
}
