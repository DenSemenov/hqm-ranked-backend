using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_helpers
{
    public static class NhlTeamsHelper
    {
        public static List<NhlTeamDTO> GetTeams()
        {
            var result = new List<NhlTeamDTO>();

            result.Add(new NhlTeamDTO("Anaheim Ducks", "https://loodibee.com/wp-content/uploads/nhl-anaheim-ducks-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Boston Bruins", "https://loodibee.com/wp-content/uploads/nhl-boston-bruins-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Buffalo Sabres", "https://loodibee.com/wp-content/uploads/nhl-buffalo-sabres-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Calgary Flames", "https://loodibee.com/wp-content/uploads/nhl-calgary-flames-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Carolina Hurricanes", "https://loodibee.com/wp-content/uploads/nhl-carolina-hurricanes-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Chicago Blackhawks", "https://loodibee.com/wp-content/uploads/nhl-chicago-blackhawks-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Colorado Avalanche", "https://loodibee.com/wp-content/uploads/nhl-colorado-avalanche-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Columbus Blue Jackets", "https://loodibee.com/wp-content/uploads/nhl-columbus-blue-jackets-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Dallas Stars", "https://loodibee.com/wp-content/uploads/nhl-dallas-stars-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Detroit Red Wings", "https://loodibee.com/wp-content/uploads/nhl-detroit-red-wings-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Edmonton Oilers", "https://loodibee.com/wp-content/uploads/nhl-edmonton-oilers-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Florida Panthers", "https://loodibee.com/wp-content/uploads/nhl-florida-panthers-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Los Angeles Kings", "https://loodibee.com/wp-content/uploads/nhl-los-angeles-kings-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Minnesota Wild", "https://loodibee.com/wp-content/uploads/nhl-minnesota-wild-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Montreal Canadiens", "https://loodibee.com/wp-content/uploads/nhl-montreal-canadiens-logo-300x300.png"));
            result.Add(new NhlTeamDTO("New Jersey Devils", "https://loodibee.com/wp-content/uploads/nhl-new-jersey-devils-logo-300x300.png"));
            result.Add(new NhlTeamDTO("New York Islanders", "https://loodibee.com/wp-content/uploads/nhl-new-york-islanders-logo-300x300.png"));
            result.Add(new NhlTeamDTO("New York Rangers", "https://loodibee.com/wp-content/uploads/nhl-new-york-rangers-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Ottawa Senators", "https://loodibee.com/wp-content/uploads/nhl-ottawa-senators-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Philadelphia Flyers", "https://loodibee.com/wp-content/uploads/nhl-philadelphia-flyers-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Pittsburgh Penguins", "https://loodibee.com/wp-content/uploads/nhl-pittsburgh-penguins-logo-300x300.png"));
            result.Add(new NhlTeamDTO("San Jose Sharks", "https://loodibee.com/wp-content/uploads/nhl-san-jose-sharks-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Seattle Kraken", "https://loodibee.com/wp-content/uploads/nhl-seattle-kraken-logo-300x300.png"));
            result.Add(new NhlTeamDTO("St. Louis Blues", "https://loodibee.com/wp-content/uploads/nhl-st-louis-blues-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Tampa Bay Lightning", "https://loodibee.com/wp-content/uploads/nhl-tampa-bay-lightning-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Toronto Maple Leafs", "https://loodibee.com/wp-content/uploads/nhl-toronto-maple-leafs-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Vancouver Canucks", "https://loodibee.com/wp-content/uploads/nhl-vancouver-canucks-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Vegas Golden Knights", "https://loodibee.com/wp-content/uploads/nhl-vegas-golden-knights-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Washington Capitals", "https://loodibee.com/wp-content/uploads/nhl-washington-capitals-logo-300x300.png"));
            result.Add(new NhlTeamDTO("Winnipeg Jets", "https://loodibee.com/wp-content/uploads/nhl-winnipeg-jets-logo-300x300.png"));

            return result;
        }
    }

    public class NhlTeamDTO
    {
        public string Name { get; set; }

        public string Url { get; set; }
        public NhlTeamDTO(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
