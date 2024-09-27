using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.DTO
{
    public class TourneyStartedDTO
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public List<TourneyStartedPlayerDTO> Players { get; set; } = new List<TourneyStartedPlayerDTO>();
    }

    public class TourneyStartedPlayerDTO
    {
        public string Name { get; set; }
        public string DiscordId { get; set; }
    }
}
